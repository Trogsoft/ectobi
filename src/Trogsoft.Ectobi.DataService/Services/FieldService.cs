using Microsoft.EntityFrameworkCore;
using System;
using System.Net.Mail;
using Trogsoft.Ectobi.Common;
using Trogsoft.Ectobi.Common.Interfaces;
using Trogsoft.Ectobi.Data;

namespace Trogsoft.Ectobi.DataService.Services
{
    public class FieldService : IFieldService
    {

        private const double UNIQUE_VALUE_MULTIPLIER = 0.925;

        private readonly ILogger<FieldService> logger;
        private readonly EctoDb db;
        private readonly IEctoMapper mapper;
        private readonly ModuleManager mm;
        private readonly IBackgroundTaskCoordinator bg;
        private readonly IWebHookService iwh;

        public FieldService(ILogger<FieldService> logger, EctoDb db, IEctoMapper mapper, ModuleManager mm, IBackgroundTaskCoordinator bg,
            IWebHookService iwh)
        {
            this.logger = logger;
            this.db = db;
            this.mapper = mapper;
            this.mm = mm;
            this.bg = bg;
            this.iwh = iwh;
        }

        private async Task<Success<T>> validateSchema<T>(string schemaTid)
        {

            if (string.IsNullOrWhiteSpace(schemaTid)) return Success<T>.Error("Schema not specified.", ErrorCodes.ERR_ARGUMENT_NULL);

            var schema = db.Schemas.SingleOrDefault(x => x.TextId == schemaTid);
            if (schema == null) return Success<T>.Error("Schema not found.", ErrorCodes.ERR_NOT_FOUND);

            return new Success<T>(true);

        }

        private async Task<Success<T>> validateField<T>(string schemaTid, string fieldTid)
        {

            if (string.IsNullOrWhiteSpace(fieldTid)) return Success<T>.Error("Field not specified.", ErrorCodes.ERR_ARGUMENT_NULL);

            var schemaResult = await validateSchema<T>(schemaTid);
            if (!schemaResult.Succeeded) return schemaResult;

            var latestVersion = db.SchemaVersions.Where(x => x.Schema.TextId == schemaTid).OrderByDescending(x => x.Version).FirstOrDefault();
            if (latestVersion == null) return Success<T>.Error("Schema has no versions.");

            var field = db.SchemaFieldVersions.SingleOrDefault(x => x.SchemaVersionId == latestVersion.Id && x.SchemaField.TextId == fieldTid);
            if (field == null) return Success<T>.Error("Field not found.", ErrorCodes.ERR_NOT_FOUND);

            return new Success<T>(true);
        }

        public async Task<Success<SchemaFieldEditModel>> GetField(string schemaTid, string fieldTid)
        {

            if (string.IsNullOrWhiteSpace(schemaTid)) return Success<SchemaFieldEditModel>.Error("Schema not specified.", ErrorCodes.ERR_ARGUMENT_NULL);
            if (string.IsNullOrWhiteSpace(fieldTid)) return Success<SchemaFieldEditModel>.Error("Field not specified.", ErrorCodes.ERR_ARGUMENT_NULL);

            var schema = db.Schemas.SingleOrDefault(x => x.TextId == schemaTid);
            if (schema == null) return Success<SchemaFieldEditModel>.Error("Schema not found.", ErrorCodes.ERR_NOT_FOUND);

            var latestVersion = db.SchemaVersions.Where(x => x.SchemaId == schema.Id).OrderByDescending(x => x.Version).FirstOrDefault();
            if (latestVersion == null) return Success<SchemaFieldEditModel>.Error("Schema has no versions.");

            var field = db.SchemaFieldVersions
                .Include(x => x.Populator)
                .Include(x => x.LookupSet)
                .SingleOrDefault(x => x.SchemaVersionId == latestVersion.Id && x.SchemaField.TextId == fieldTid);
            if (field == null) return Success<SchemaFieldEditModel>.Error("Field not found.", ErrorCodes.ERR_NOT_FOUND);

            return new Success<SchemaFieldEditModel>(mapper.Map<SchemaFieldEditModel>(field));

        }

        public async Task<Success<SchemaFieldModel>> EditField(string schemaTid, string fieldTid, SchemaFieldEditModel model)
        {
            if (model == null) return Success<SchemaFieldModel>.Error("Model cannot be null.", ErrorCodes.ERR_ARGUMENT_NULL);
            var field = db.SchemaFieldVersions.SingleOrDefault(x => x.TextId == model.TextId);

            // Values that require a new version
            throw new NotImplementedException();

        }

        public async Task<Success<List<SchemaFieldModel>>> GetVersionFields(string schemaTid, int version)
        {

            var schemaVersion = db.SchemaVersions.SingleOrDefault(x => x.Schema.TextId == schemaTid && x.Version == version);
            if (schemaVersion == null) return Success<List<SchemaFieldModel>>.Error("Schema version not found.", ErrorCodes.ERR_NOT_FOUND);

            var fields = await db.SchemaFieldVersions
                .Where(x => x.SchemaVersion.Schema.TextId == schemaTid && x.SchemaVersion.Version == version)
                .ToListAsync();
            var list = fields.Select(x => mapper.Map<SchemaFieldModel>(x)).ToList();
            return new Success<List<SchemaFieldModel>>(list);

        }

        public async Task<Success<List<SchemaFieldModel>>> GetFields(string schemaTid)
        {
            var fields = await db.SchemaFields.Where(x => x.Schema.TextId == schemaTid).ToListAsync();
            var list = fields.Select(x => mapper.Map<SchemaFieldModel>(x)).ToList();
            return new Success<List<SchemaFieldModel>>(list);
        }

        public async Task<Success> DeleteField(string schemaTid, string fieldName)
        {

            if (string.IsNullOrWhiteSpace(schemaTid)) return Success.Error("SchemaTid cannot be null.", ErrorCodes.ERR_ARGUMENT_NULL);
            if (string.IsNullOrWhiteSpace(fieldName)) return Success.Error("fieldName cannot be null.", ErrorCodes.ERR_ARGUMENT_NULL);

            var schema = db.Schemas.Include(x => x.SchemaFields).SingleOrDefault(x => x.TextId == schemaTid);
            if (schema == null) return Success.Error("Schema not found.", ErrorCodes.ERR_NOT_FOUND);

            var field = schema.SchemaFields.SingleOrDefault(f => f.TextId == fieldName);
            if (field == null) return Success.Error("Field not found.", ErrorCodes.ERR_NOT_FOUND);

            db.Remove(field);

            try
            {
                await db.SaveChangesAsync();
                await iwh.Dispatch(WebHookEventType.FieldDeleted, new { schema = schemaTid, field = fieldName });
            }
            catch (Exception ex)
            {
                return Success.Error(ex.Message, ErrorCodes.ERR_UNSPECIFIED_ERROR);
            }

            return new Success(true);

        }

        public async Task<Success<SchemaFieldModel>> CreateField(string schemaTid, SchemaFieldEditModel model)
        {

            if (string.IsNullOrWhiteSpace(schemaTid)) return Success<SchemaFieldModel>.Error("SchemaTid cannot be null.", ErrorCodes.ERR_ARGUMENT_NULL);
            if (model == null) return Success<SchemaFieldModel>.Error("Model cannot be null.", ErrorCodes.ERR_ARGUMENT_NULL);
            if (string.IsNullOrWhiteSpace(model.Name)) return Success<SchemaFieldModel>.Error("Model.Name cannot be null.", ErrorCodes.ERR_ARGUMENT_NULL);

            var schema = db.Schemas.Include(x => x.SchemaFields).SingleOrDefault(x => x.TextId == schemaTid);
            if (schema == null) return Success<SchemaFieldModel>.Error("Schema not found.", ErrorCodes.ERR_NOT_FOUND);

            if (schema.SchemaFields.Any(x => x.Name == model.Name)) return Success<SchemaFieldModel>.Error("Field already exists.", ErrorCodes.ERR_FIELD_ALREADY_EXISTS);

            var newField = mapper.Map<SchemaField>(model);
            newField.TextId = db.GetTextId<SchemaField>($"{model.Name}");

            var transaction = db.Database.BeginTransaction();

            var latestVersion = db.SchemaVersions.Where(x => x.SchemaId == schema.Id).OrderByDescending(x => x.Version).FirstOrDefault();
            var fieldVersion = mapper.Map<SchemaFieldVersion>(newField);
            fieldVersion.SchemaField = newField;

            if (latestVersion != null)
            {
                if (!string.IsNullOrWhiteSpace(model.Populator) && mm.PopulatorExists(model.Populator))
                    fieldVersion.PopulatorId = mm.GetPopulatorDatabaseId(model.Populator);

                if (!string.IsNullOrWhiteSpace(model.ModelTid))
                {
                    var dbModel = await db.Models.SingleOrDefaultAsync(x => x.TextId == model.ModelTid);
                    if (dbModel != null)
                    {
                        fieldVersion.ModelId = dbModel.Id;
                        fieldVersion.ModelField = model.ModelField;
                    } 
                    // todo: what to do if the model doesn't exist?
                }

                latestVersion.Fields.Add(fieldVersion);
            }

            schema.SchemaFields.Add(newField);
            try
            {
                await db.SaveChangesAsync();
                await iwh.Dispatch(WebHookEventType.FieldCreated, mapper.Map<SchemaFieldModel>(fieldVersion));
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return Success<SchemaFieldModel>.Error(ex.Message, ErrorCodes.ERR_UNSPECIFIED_ERROR);
            }

            transaction.Commit();

            bg.Enqueue<IFieldService>(x => x.PopulateField(newField.Id));

            var returnValue = mapper.Map<SchemaFieldModel>(newField);
            return new Success<SchemaFieldModel>(returnValue);

        }

        public void PopulateField(long fieldId)
        {

            var field = db.SchemaFields.SingleOrDefault(x => x.Id == fieldId);
            if (field == null) throw new Exception("Field not found.");

            if (field.Type == SchemaFieldType.Populator)
            {

                var fv = db.SchemaFieldVersions.Include(x => x.Populator).Where(x => x.SchemaFieldId == fieldId).OrderByDescending(x => x.SchemaVersion.Version).FirstOrDefault();

                foreach (var record in db.Records.Where(x => x.Batch.SchemaVersion.Fields.Any(y => y.SchemaFieldId == fieldId)))
                {

                    var populator = mm.GetPopulator(fv.Populator.Name);
                    var recordField = db.Values.SingleOrDefault(x => x.RecordId == record.Id && x.SchemaFieldVersion.SchemaFieldId == fieldId);
                    if (recordField == null)
                        record.Values.Add(new Value
                        {
                            SchemaFieldVersionId = fv.Id,
                            RawValue = populator.GetString()
                        });

                }

                db.SaveChanges();

            }

        }

        public void AutoDetectSchemaFieldParameters(SchemaFieldEditModel x)
        {

            if (x == null) return;

            var nonNullValueCount = x.RawValues.Count(y => y != null);

            //if (nonNullValueCount > (x.RawValues.Count * UNIQUE_VALUE_MULTIPLIER))
            //    x.Flags |= SchemaFieldFlags.RequiredAtImport;

            // Detect decimal values
            List<decimal?> decimalValues = new();
            x.RawValues.ForEach(x =>
            {
                if (Decimal.TryParse(x, out decimal val))
                    decimalValues.Add(val);
            });

            // Detect whole numbers
            List<int?> numericValues = new();
            x.RawValues.ForEach(x =>
            {
                if (Int32.TryParse(x, out int val))
                    numericValues.Add(val);
            });

            if (decimalValues.Count == nonNullValueCount)
                x.Type = SchemaFieldType.Decimal;

            if (numericValues.Count == nonNullValueCount)
                x.Type = SchemaFieldType.Integer;

            // Detect dates
            List<DateTime?> dateTimes = new List<DateTime?>();
            x.RawValues.ForEach(x =>
            {
                if (DateTime.TryParse(x, out DateTime val))
                    dateTimes.Add(val);
            });

            if (dateTimes.Count == nonNullValueCount)
                x.Type = SchemaFieldType.DateTime;

            // Emails
            //List<string> emails = new List<string>();
            //x.RawValues.ForEach(x =>
            //{
            //    try
            //    {
            //        new MailAddress(x);
            //        emails.Add(x);
            //    }
            //    catch
            //    {
            //        // Don't care
            //    }
            //});

            //if (emails.Count == nonNullValueCount)
            //{
            //    x.Type = SchemaFieldType.Text;
            //    // todo: Add a validator
            //}


            // Detect a set
            var uniqueNonNullValues = x.RawValues.Where(y => !string.IsNullOrWhiteSpace(y)).Distinct();
            var maxUniqueValues = 50;
            if (uniqueNonNullValues.Count() <= maxUniqueValues)
            {
                x.Type = SchemaFieldType.Set;
            }

            // Free text
            if (uniqueNonNullValues.Count() > nonNullValueCount * UNIQUE_VALUE_MULTIPLIER)
            {
                x.Type = SchemaFieldType.Text;
            }

        }

    }
}
