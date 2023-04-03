using Hangfire.Server;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.Xml;
using Trogsoft.Ectobi.Common;
using Trogsoft.Ectobi.Common.Interfaces;
using Trogsoft.Ectobi.Data;

namespace Trogsoft.Ectobi.DataService.Services
{
    public class BatchService : IBatchService
    {
        private readonly ILogger<BatchService> logger;
        private readonly EctoDb db;
        private readonly IEctoMapper mapper;
        private readonly IBackgroundTaskCoordinator bg;
        private readonly ModuleManager mm;
        private readonly IWebHookService iwh;

        public BatchService(ILogger<BatchService> logger, EctoDb db, IEctoMapper mapper, IBackgroundTaskCoordinator bg, ModuleManager mm, IWebHookService iwh)
        {
            this.logger = logger;
            this.db = db;
            this.mapper = mapper;
            this.bg = bg;
            this.mm = mm;
            this.iwh = iwh;
        }

        public async Task<Success<BatchModel>> CreateEmptyBatch(BatchModel model)
        {

            if (model == null) return Success<BatchModel>.Error("Model cannot be null.", ErrorCodes.ERR_ARGUMENT_NULL);
            if (string.IsNullOrWhiteSpace(model.Name)) return Success<BatchModel>.Error("Model.Name cannot be null.", ErrorCodes.ERR_ARGUMENT_NULL);

            var schema = db.Schemas.Include(x=>x.Versions).SingleOrDefault(x => x.TextId == model.SchemaTid);
            if (schema == null) return Success<BatchModel>.Error("Schema not found.", ErrorCodes.ERR_NOT_FOUND);
            if (!schema.Versions.Any()) return Success<BatchModel>.Error("No schema versions exist.", ErrorCodes.ERR_NOT_FOUND);

            var newModel = mapper.Map<Batch>(model);
            newModel.SchemaVersionId = schema.Versions.OrderByDescending(x => x.Version).FirstOrDefault()!.Id;
            newModel.TextId = Guid.NewGuid().ToString();

            db.Batches.Add(newModel);
            await db.SaveChangesAsync();
            iwh.Dispatch(WebHookEventType.BatchCreated, mapper.Map<BatchModel>(newModel)).Wait();

            return new Success<BatchModel>(mapper.Map<BatchModel>(newModel));

        }

        public async Task<Success<BackgroundTaskInfo>> ImportBatch(ImportBatchModel model)
        {
            if (model == null) return Success<BackgroundTaskInfo>.Error("Model cannot be null", ErrorCodes.ERR_ARGUMENT_NULL);

            var schema = db.Schemas.SingleOrDefault(x => x.TextId == model.SchemaTid);
            if (schema == null) return Success<BackgroundTaskInfo>.Error("Schema not found.", ErrorCodes.ERR_NOT_FOUND);

            BackgroundTaskInfo job = new BackgroundTaskInfo();
            bg.Enqueue<IBatchService>(x => x.BackgroundImportBatch(job, model));            
            return new Success<BackgroundTaskInfo>(job);
        }

        public async Task<Success<List<BatchModel>>> GetBatches(string schemaTid)
        {

            if (string.IsNullOrWhiteSpace(schemaTid)) return Success<List<BatchModel>>.Error("Schema not specified.", ErrorCodes.ERR_ARGUMENT_NULL);

            var schema = db.Schemas.SingleOrDefault(x => x.TextId == schemaTid);
            if (schema == null) return Success<List<BatchModel>>.Error("Schema not found.", ErrorCodes.ERR_NOT_FOUND);

            List<BatchModel> bm = new List<BatchModel>();
            var batches = await db.Batches.Where(x => x.SchemaVersion.SchemaId == schema.Id).ToListAsync();
            bm.AddRange(batches.Select(x => mapper.Map<BatchModel>(x)));

            return new Success<List<BatchModel>>(bm);

        }

        public Success BackgroundImportBatch(BackgroundTaskInfo job, ImportBatchModel model)
        {

            if (model == null) return Success.Error("Model cannot be null", ErrorCodes.ERR_ARGUMENT_NULL);

            var schema = db.Schemas.SingleOrDefault(x => x.TextId == model.SchemaTid);
            if (schema == null) return Success.Error("Schema not found.", ErrorCodes.ERR_NOT_FOUND);

            if (!string.IsNullOrWhiteSpace(model.BatchSource))
            {
                if (db.Batches.Any(x => x.Source == model.BatchSource && x.SchemaVersion.SchemaId == schema.Id))
                    return Success.Error("Batch has previously been uploaded.", ErrorCodes.ERR_BATCH_EXISTS);
            }

            if (!string.IsNullOrWhiteSpace(model.BatchName))
            {
                if (db.Batches.Any(x => x.Name == model.BatchName && x.SchemaVersion.SchemaId == schema.Id))
                    return Success.Error("Batch name is in use.", ErrorCodes.ERR_BATCH_EXISTS);
            }

            // Always import into the maximum version unless otherwise specified
            var schemaVersion = db.SchemaVersions.OrderByDescending(x => x.Created).FirstOrDefault(x => x.SchemaId == schema.Id);
            if (schemaVersion == null)
                return Success.Error("No schema versions defined.");

            var fields = db.SchemaFieldVersions.Where(x => x.SchemaVersionId == schemaVersion.Id).ToList(); // Cache an in-memory copy of the fields for future reference

            List<ValidationResult> validationResults = new();
            validationResults.AddRange(ValidateBatchForImport(model, fields));

            if (validationResults.Any(x => x.Failed))
            {
                validationResults.Where(x => x.Failed).ToList().ForEach(x => logger.LogError(x.Message));
                return new Success(false);
            }

            var transaction = db.Database.BeginTransaction();

            var batch = new Batch
            {
                Created = DateTime.Now,
                Source = model.BatchSource,
                Name = model.BatchName,
                SchemaVersionId = schemaVersion.Id,
                Flags = BatchFlags.Importing
            };

            db.Batches.Add(batch);
            db.SaveChanges();

            foreach (var row in model.ValueMap.Rows)
            {
                var i = 0;
                var record = new Record();
                foreach (var header in model.ValueMap.Headings)
                {

                    var field = fields.SingleOrDefault(x => x.Name == header);
                    if (field == null)
                        throw new Exception("Field not found.");

                    record.Values.Add(new Value
                    {
                        SchemaFieldVersionId = field.Id,
                        RawValue = row[i]
                    });

                    i++;
                }
                batch.Records.Add(record);
            }

            batch.Flags = BatchFlags.Processing;
            db.SaveChanges();

            iwh.Dispatch(WebHookEventType.BatchCreated, mapper.Map<BatchModel>(batch)).Wait();

            transaction.Commit();

            bg.Enqueue<IBatchService>(x => x.ExecutePopulators(batch.Id));

            return new Success(true);

        }

        public Success ExecutePopulators(long id)
        {

            var batch = db.Batches
                .Include(x => x.SchemaVersion)
                .ThenInclude(x => x.Fields)
                .ThenInclude(x => x.Populator)
                .SingleOrDefault(x => x.Id == id);
            if (batch == null) return new Success(true, "There was no work to do.");

            var count = 0;
            var records = db.Records.Where(x => x.BatchId == batch.Id).Select(x => x.Id).ToList();
            foreach (var recordId in records)
            {
                foreach (var field in batch.SchemaVersion.Fields.Where(x => x.Populator != null))
                {
                    var populator = mm.GetPopulator(field.Populator!.Name);
                    if (populator == null) continue;

                    var recordField = db.Values.SingleOrDefault(x => x.RecordId == recordId && x.SchemaFieldVersionId == field.Id);
                    if (recordField == null)
                    {
                        recordField = new Value
                        {
                            RecordId = recordId,
                            SchemaFieldVersionId = field.Id,
                            RawValue = populator.GetString()
                        };
                        db.Values.Add(recordField);
                    }
                    else
                    {
                        recordField.RawValue = populator.GetString();
                    }

                }

                count++;
                if (count % 1000 == 0)
                {
                    db.SaveChanges();
                    count = 0;
                }

            }
            db.SaveChanges();

            return new Success();

        }

        private List<ValidationResult> ValidateBatchForImport(ImportBatchModel model, List<SchemaFieldVersion> fields)
        {

            List<ValidationResult> results = new();

            foreach (var field in fields)
            {

                // If the field is required at import time
                if (field.Flags.HasFlag(SchemaFieldFlags.RequiredAtImport))
                {

                    // And it's not contained within the value map
                    if (!model.ValueMap.Headings.Contains(field.Name!, StringComparer.CurrentCultureIgnoreCase))
                    {
                        // Record an error and skip to the next field
                        results.Add(new ValidationResult { Failed = true, Message = $"The required field {field.Name} is not specified." });
                        continue;
                    }

                    // Check every row has a corresponding value.
                    var headingIndex = model.ValueMap.Headings.FindIndex(x => x.Equals(field.Name!, StringComparison.CurrentCultureIgnoreCase));
                    var allValues = model.ValueMap.Rows.Select(y => y[headingIndex]).ToList();
                    if (allValues.Any(x => string.IsNullOrWhiteSpace(x)))
                    {
                        results.Add(new ValidationResult { Failed = true, Message = $"The required field {field.Name} is empty on at least one row." });
                    }

                }

            }

            return results;

        }
    }
}
