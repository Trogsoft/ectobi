using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.CustomUI;
using Hangfire.Annotations;
using Jint;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using Trogsoft.Ectobi.Common;
using Trogsoft.Ectobi.Common.Interfaces;
using Trogsoft.Ectobi.Data;
using Trogsoft.Ectobi.DataService.Data;
using Trogsoft.Ectobi.DataService.Interfaces;
using Trogsoft.Ectobi.DataService.Validation;

namespace Trogsoft.Ectobi.DataService.Services
{
    public class FieldService : IFieldService, IFieldBackgroundService
    {

        private const double UNIQUE_VALUE_MULTIPLIER = 0.925;

        private readonly ILogger<FieldService> logger;
        private readonly EctoDb db;
        private readonly IEctoMapper mapper;
        private readonly ModuleManager mm;
        private readonly IBackgroundTaskCoordinator backgroundService;
        private readonly IEctoData data;
        private readonly IPopulatorService populatorService;
        private readonly IScriptingService scriptingService;

        public FieldService(ILogger<FieldService> logger, EctoDb db, IEctoMapper mapper, ModuleManager mm, IBackgroundTaskCoordinator bg,
            IEctoData data, IPopulatorService ps, IScriptingService scriptingService)
        {
            this.logger = logger;
            this.db = db;
            this.mapper = mapper;
            this.mm = mm;
            this.backgroundService = bg;
            this.data = data;
            this.populatorService = ps;
            this.scriptingService = scriptingService;
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

        public async Task<Success<List<SchemaFieldModel>>> GetVersionFields(string schemaTid, int version = 0)
        {

            SchemaVersion? schemaVersion;
            if (version == 0) // The latest version
                schemaVersion = db.SchemaVersions.Where(x => x.Schema.TextId == schemaTid).OrderByDescending(x => x.Version).FirstOrDefault();
            else
                schemaVersion = db.SchemaVersions.SingleOrDefault(x => x.Schema.TextId == schemaTid && x.Version == version);

            if (schemaVersion == null) return Success<List<SchemaFieldModel>>.Error("Schema version not found.", ErrorCodes.ERR_NOT_FOUND);

            version = schemaVersion.Version;
            var fields = await db.SchemaFieldVersions
                .Include(x => x.Model)
                .Where(x => x.SchemaVersion.Schema.TextId == schemaTid && x.SchemaVersion.Version == version)
                .ToListAsync();
            var list = fields.Select(x => mapper.Map<SchemaFieldModel>(x)).ToList();
            return new Success<List<SchemaFieldModel>>(list);

        }

        public async Task<Success<List<SchemaFieldModel>>> GetFields(string schemaTid)
        {
            var validator = EctoModelValidator.CreateValidator<string>(db)
                .WithModel(schemaTid)
                .Value(schemaTid, nameof(schemaTid)).NotNullOrWhiteSpace()
                .Entity<Schema>(x => x.TextId == schemaTid).MustExist();
            if (!validator.Validate()) return validator.GetResult<List<SchemaFieldModel>>();

            var fields = await db.SchemaFields.Where(x => x.Schema.TextId == schemaTid).ToListAsync();
            var list = fields.Select(x => mapper.Map<SchemaFieldModel>(x)).ToList();
            return new Success<List<SchemaFieldModel>>(list);
        }

        Success IFieldBackgroundService.DeleteField(BackgroundTaskInfo job, string schemaTid, string fieldTid, int version)
        {
            try
            {
                var result = data.Field.DeleteField(schemaTid, fieldTid, version).Result;                
                return new Success(result);
            }
            catch (Exception err)
            {
                return Success.Error(err.Message, ErrorCodes.ERR_UNSPECIFIED_ERROR);
            }
        }

        public async Task<Success<BackgroundTaskInfo>> DeleteField(string schemaTid, string fieldName, int version = 0)
        {

            if (string.IsNullOrWhiteSpace(schemaTid)) return Success<BackgroundTaskInfo>.Error("SchemaTid cannot be null.", ErrorCodes.ERR_ARGUMENT_NULL);
            if (string.IsNullOrWhiteSpace(fieldName)) return Success<BackgroundTaskInfo>.Error("fieldName cannot be null.", ErrorCodes.ERR_ARGUMENT_NULL);

            BackgroundTaskInfo job = new BackgroundTaskInfo($"Deleting {fieldName}");
            backgroundService.Enqueue<IFieldBackgroundService>(x => x.DeleteField(job, schemaTid, fieldName, version));
            return new Success<BackgroundTaskInfo>(job);

        }

        public async Task<Success<SchemaFieldModel>> UpdateField(string schemaTid, SchemaFieldEditModel model)
        {

            var validator = EctoModelValidator.CreateValidator<SchemaFieldEditModel>(db)
                .WithModel(model)
                .Value(schemaTid, nameof(schemaTid)).NotNullOrWhiteSpace()
                .Property(x => x.Name).NotNullOrWhiteSpace()
                .Property(x => x.Id).MustBeGreaterThan(0)
                .Entity<Schema>(x => x.TextId == schemaTid).MustExist();

            if (!validator.Validate()) return validator.GetResult<SchemaFieldModel>();

            var dbField = await db.SchemaFieldVersions.SingleOrDefaultAsync(x => x.Id == model.Id);
            if (dbField == null) return Success<SchemaFieldModel>.Error("Field not found.", ErrorCodes.ERR_NOT_FOUND);

            dbField.Name = model.Name;
            dbField.Description = model.Description;
            dbField.Type = model.Type;

            if (!string.IsNullOrWhiteSpace(model.ModelName))
            {
                var m = db.Models.SingleOrDefault(x => x.TextId == model.ModelName);
                if (m != null)
                {
                    dbField.Model = m;
                    dbField.ModelField = model.ModelField;
                }
            }

            bool applyLookupValues = false;
            if (model.Type == SchemaFieldType.Set)
            {
                if (model.Set == "__create__")
                {
                    var ls = new LookupSetModel { Name = model.Name + " Values", SchemaTid = schemaTid };
                    var values = await db.Values
                        .Where(x => x.SchemaFieldVersionId == dbField.Id)
                        .Select(x => x.RawValue)
                        .Distinct()
                        .ToListAsync();

                    ls.Values = values.Select((x, i) => new LookupSetValueModel { Name = x, Value = i + 1 }).ToList();

                    var lookup = await data.Lookup.CreateLookupSet(ls);
                    dbField.LookupSetId = lookup.Id;

                    applyLookupValues = true;

                }
                else
                {
                    if (await data.Lookup.LookupSetExists(model.Set))
                    {
                        dbField.LookupSetId = await data.Lookup.GetLookupSetId(model.Set);
                    }
                    else
                    {
                        return Success<SchemaFieldModel>.Error("Lookup set not found", ErrorCodes.ERR_NOT_FOUND);
                    }
                }
            }

            await db.SaveChangesAsync();

            if (applyLookupValues)
                backgroundService.Enqueue<IFieldService>(x => x.ApplyLookupValuesToField(schemaTid, model.TextId));

            return new Success<SchemaFieldModel>(mapper.Map<SchemaFieldModel>(dbField));

        }

        public async Task<Success<SchemaFieldModel>> CreateField(string schemaTid, SchemaFieldEditModel model)
        {

            var validator = EctoModelValidator.CreateValidator<SchemaFieldEditModel>(db)
                .WithModel(model)
                .Value(schemaTid, nameof(schemaTid)).NotNullOrWhiteSpace()
                .Property(x => x.Name).NotNullOrWhiteSpace()
                .Entity<Schema>(x => x.TextId == schemaTid).MustExist()
                .Entity<SchemaField>(x => x.Name == model.Name && x.Schema.TextId == schemaTid).MustNotExist();

            if (model.LookupTid != null && model.Type == SchemaFieldType.Set)
                validator.Entity<LookupSet>(x => x.TextId == model.LookupTid).MustExist();

            if (!validator.Validate()) return validator.GetResult<SchemaFieldModel>();

            // Additional validation
            if (model.Type == 0) return Success<SchemaFieldModel>.Error("Field model must have Type property.", ErrorCodes.ERR_ARGUMENT_INVALID);

            model.SchemaTid = schemaTid;

            var transaction = data.BeginTransaction();

            SchemaField? rootField = null;
            SchemaFieldModel? versionField = null;
            try
            {
                rootField = await data.Field.CreateRootField(model);
                versionField = await data.Field.CreateVersionField(model);

                if (model.Type == SchemaFieldType.Populator && !string.IsNullOrWhiteSpace(model.Populator))
                    await data.Field.SetFieldPopulator(versionField, model.Populator);

                if (!string.IsNullOrWhiteSpace(model.ModelName))
                    await data.Field.SetFieldModel(versionField, model.ModelName, model.ModelField);

                if (model.Type == SchemaFieldType.Formula && !string.IsNullOrWhiteSpace(model.Formula))
                    await data.Field.SetFormula(versionField, model.Formula);

                data.CommitTransaction();

                var job = new BackgroundTaskInfo($"Populating field {rootField.Name}");
                backgroundService.Enqueue<IFieldBackgroundService>(x => x.PopulateField(job, rootField.Id));

                return new Success<SchemaFieldModel>(versionField);
            }
            catch (Exception ex)
            {
                data.RollbackTransaction();
                return Success<SchemaFieldModel>.Error(ex.Message, ErrorCodes.ERR_UNSPECIFIED_ERROR);
            }

        }

        object? GetTypedValueFromField(Value value)
        {
            if (value == null) return null;
            if (value.SchemaFieldVersion == null || string.IsNullOrWhiteSpace(value.SchemaFieldVersion.TextId)) return null;
            if (value.BoolValue != null) return value.BoolValue.Value;
            else if (value.DecimalValue.HasValue) return value.DecimalValue.Value;
            else if (value.IntegerValue.HasValue) return value.IntegerValue.Value;
            else return value.RawValue;
        }

        void IFieldBackgroundService.PopulateField(BackgroundTaskInfo job, long fieldId)
        {

            backgroundService.TaskBegun(job);
            backgroundService.TaskStatusChanged(job, "Loading field from database");

            var field = db.SchemaFields.SingleOrDefault(x => x.Id == fieldId);
            if (field == null)
            {
                backgroundService.TaskFailed(job);
                throw new Exception("Field not found.");
            }

            var fieldVersion = db.SchemaFieldVersions.Include(x => x.Populator).Where(x => x.SchemaFieldId == fieldId).OrderByDescending(x => x.SchemaVersion.Version).FirstOrDefault();
            if (fieldVersion == null)
            {
                backgroundService.TaskFailed(job);
                throw new Exception("Schema Field Version not found.");
            }

            var records = db.Records.Where(x => x.Batch.SchemaVersion.Fields.Any(y => y.SchemaFieldId == fieldId) 
                && !x.Values.Any(x=>x.SchemaFieldVersion.SchemaFieldId == fieldId));
            var total = records.Count();
            var count = 0;

            if (field.Type == SchemaFieldType.Populator)
            {

                backgroundService.TaskStatusChanged(job, $"Running populator {fieldVersion.Populator} on {fieldVersion.Name}");

                foreach (var record in records)
                {

                    backgroundService.TaskProgressChanged(job, total, count);

                    count++;

                    var recordField = db.Values.SingleOrDefault(x => x.RecordId == record.Id && x.SchemaFieldVersion.SchemaFieldId == fieldId);
                    if (recordField == null)
                    {
                        var val = populatorService.GetPopulatedValue(fieldVersion);
                        record.Values.Add(val);
                    }

                }

                backgroundService.TaskStatusChanged(job, "Saving");
                db.SaveChanges();

            }
            else if (field.Type == SchemaFieldType.Script)
            {

                // todo: run scripts.

            }
            else if (field.Type == SchemaFieldType.Formula)
            {

                var formula = fieldVersion.Formula;
                if (string.IsNullOrWhiteSpace(formula)) return;

                backgroundService.TaskStatusChanged(job, $"Resolving formula on {fieldVersion.Name}");

                foreach (var record in records)
                {

                    backgroundService.TaskProgressChanged(job, total, count);
                    count++;

                    var fields = db.Values.Include(x => x.SchemaFieldVersion).Where(x => x.RecordId == record.Id).Where(x => x.SchemaFieldVersion != null).ToList();
                    var recordField = fields.SingleOrDefault(x => x.SchemaFieldVersion.SchemaFieldId == fieldId);
                    if (recordField == null)
                    {

                        var val = new Value
                        {
                            SchemaFieldVersionId = fieldVersion.Id
                        };

                        var context = new Dictionary<string, object>();
                        var i = 0;
                        foreach (var f in fields)
                            context.Add(f.SchemaFieldVersion.TextId ?? $"Property{i++}", GetTypedValueFromField(f)!);

                        var parameters = new Dictionary<string, object>() {
                            { "record", context }
                        };
                        var result = scriptingService.ExecuteFormula(formula, parameters)
                            .GetRecordValue(val);

                        record.Values.Add(val);

                    }

                }

                backgroundService.TaskStatusChanged(job, "Saving");
                db.SaveChanges();

            }

            backgroundService.TaskCompleted(job);

        }

        void IFieldBackgroundService.AutoDetectSchemaFieldParameters(SchemaFieldEditModel x)
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

        public void ApplyLookupValuesToField(string schemaTid, string? textId)
        {

            var field = db.SchemaFields.SingleOrDefault(x => x.Schema.TextId == schemaTid && x.TextId == textId);
            if (field == null) return;

            var latestFieldVersion = db.SchemaFieldVersions.Where(x => x.SchemaFieldId == field.Id).OrderByDescending(x => x.SchemaVersion.Version).FirstOrDefault();
            if (latestFieldVersion == null) return;
            if (latestFieldVersion.Type != SchemaFieldType.Set) return;

            var lookupValues = db.LookupSetValues.Where(x => x.LookupSetId == latestFieldVersion.LookupSetId).ToList();
            var fieldValues = db.Values.Where(x => x.SchemaFieldVersionId == latestFieldVersion.Id);

            foreach (var value in fieldValues)
            {
                var lv = lookupValues.SingleOrDefault(x => x.Name == value.RawValue);
                if (lv == null) return;

                value.IntegerValue = lv.NumericValue;
            }

            db.SaveChanges();

        }
    }
}
