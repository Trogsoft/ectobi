using Hangfire.Annotations;
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
    public class FieldService : IFieldService, IInternalFieldService
    {

        private const double UNIQUE_VALUE_MULTIPLIER = 0.925;

        private readonly ILogger<FieldService> logger;
        private readonly EctoDb db;
        private readonly IEctoMapper mapper;
        private readonly ModuleManager mm;
        private readonly IBackgroundTaskCoordinator bg;
        private readonly IEctoData data;

        public FieldService(ILogger<FieldService> logger, EctoDb db, IEctoMapper mapper, ModuleManager mm, IBackgroundTaskCoordinator bg,
            IEctoData data)
        {
            this.logger = logger;
            this.db = db;
            this.mapper = mapper;
            this.mm = mm;
            this.bg = bg;
            this.data = data;
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

        public async Task<Success> DeleteField(string schemaTid, string fieldName, int version = 0)
        {

            if (string.IsNullOrWhiteSpace(schemaTid)) return Success.Error("SchemaTid cannot be null.", ErrorCodes.ERR_ARGUMENT_NULL);
            if (string.IsNullOrWhiteSpace(fieldName)) return Success.Error("fieldName cannot be null.", ErrorCodes.ERR_ARGUMENT_NULL);

            try
            {
                await data.Field.DeleteField(schemaTid, fieldName, version);
                return new Success(true);
            }
            catch (Exception err)
            {
                return Success.Error(err.Message, ErrorCodes.ERR_UNSPECIFIED_ERROR);
            }

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
                bg.Enqueue<IFieldService>(x => x.ApplyLookupValuesToField(schemaTid, model.TextId));

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

                bg.Enqueue<IInternalFieldService>(x => x.PopulateField(rootField.Id));

                data.CommitTransaction();
                return new Success<SchemaFieldModel>(versionField);
            }
            catch (Exception ex)
            {
                data.RollbackTransaction();
                return Success<SchemaFieldModel>.Error(ex.Message, ErrorCodes.ERR_UNSPECIFIED_ERROR);
            }

        }

        void IInternalFieldService.PopulateField(long fieldId)
        {

            var field = db.SchemaFields.SingleOrDefault(x => x.Id == fieldId);
            if (field == null) throw new Exception("Field not found.");

            if (field.Type == SchemaFieldType.Populator)
            {

                var fv = db.SchemaFieldVersions.Include(x => x.Populator).Where(x => x.SchemaFieldId == fieldId).OrderByDescending(x => x.SchemaVersion.Version).FirstOrDefault();

                foreach (var record in db.Records.Where(x => x.Batch.SchemaVersion.Fields.Any(y => y.SchemaFieldId == fieldId)))
                {

                    var populator = mm.GetPopulator(fv.Populator.TextId);
                    var rt = populator.GetReturnType();

                    var pconfig = fv.PopulatorConfiguration;
                    var poptions = new Dictionary<string, string>();

                    if (!string.IsNullOrWhiteSpace(pconfig))
                        poptions = JsonConvert.DeserializeObject<Dictionary<string, string>>(pconfig);

                    var recordField = db.Values.SingleOrDefault(x => x.RecordId == record.Id && x.SchemaFieldVersion.SchemaFieldId == fieldId);
                    if (recordField == null)
                    {
                        var val = new Value
                        {
                            SchemaFieldVersionId = fv.Id
                        };

                        switch (rt)
                        {
                            case PopulatorReturnType.Integer:
                                val.IntegerValue = populator.GetInteger(poptions);
                                break;

                            case PopulatorReturnType.Decimal:
                                val.DecimalValue = populator.GetDecimal(poptions);
                                break;

                            case PopulatorReturnType.String:
                            default:
                                val.RawValue = populator.GetString(poptions);
                                break;
                        }

                        record.Values.Add(val);
                    }

                }

                db.SaveChanges();

            }
            else if (field.Type == SchemaFieldType.Script)
            {

                // todo: run scripts.

            }

        }

        void IInternalFieldService.AutoDetectSchemaFieldParameters(SchemaFieldEditModel x)
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
