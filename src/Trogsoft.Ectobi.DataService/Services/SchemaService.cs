using Hangfire.Annotations;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using Trogsoft.Ectobi.Common;
using Trogsoft.Ectobi.Common.Interfaces;
using Trogsoft.Ectobi.Data;
using Trogsoft.Ectobi.Data.Migrations;

namespace Trogsoft.Ectobi.DataService.Services
{
    public class SchemaService : ISchemaService
    {
        private const double UNIQUE_VALUE_MULTIPLIER = 0.98;
        private readonly ILogger<SchemaService> logger;
        private readonly EctoDb db;
        private readonly IEctoMapper mapper;
        private readonly IFieldService fields;
        private readonly ILookupService lookup;
        private readonly IWebHookService iwh;
        private readonly IFileTranslatorService fts;

        public SchemaService(ILogger<SchemaService> logger, EctoDb db, IEctoMapper mapper, IFieldService fields,
            ILookupService lookup, IWebHookService iwh, IFileTranslatorService fts)
        {
            this.logger = logger;
            this.db = db;
            this.mapper = mapper;
            this.fields = fields;
            this.lookup = lookup;
            this.iwh = iwh;
            this.fts = fts;
        }

        public async Task<Success<List<SchemaModel>>> GetSchemas(bool includeDetails = false)
        {
            List<SchemaModel> schemaModels = new List<SchemaModel>();
            var schemaQuery = db.Schemas.AsQueryable();
            if (includeDetails)
                schemaQuery = schemaQuery.Include(x => x.SchemaFields);

            foreach (var schema in await schemaQuery.ToListAsync())
                schemaModels.Add(mapper.Map<SchemaModel>(schema));

            return new Success<List<SchemaModel>>(schemaModels);
        }

        public async Task<Success<SchemaModel>> CreateSchema(SchemaEditModel model)
        {

            if (model == null) return Success<SchemaModel>.Error("Schema cannot be null.", ErrorCodes.ERR_ARGUMENT_NULL);
            if (model.Id > 0) return Success<SchemaModel>.Error("Schema already exists.", ErrorCodes.ERR_SCHEMA_ALREADY_EXISTS);
            if (string.IsNullOrWhiteSpace(model.Name)) return Success<SchemaModel>.Error("Schema name was not specified.", ErrorCodes.ERR_REQUIRED_VALUE_EMPTY);

            if (!model.Overwrite)
            {
                if (db.Schemas.Any(x => x.Name == model.Name)) return Success<SchemaModel>.Error("Schema already exists.", ErrorCodes.ERR_SCHEMA_ALREADY_EXISTS);
            }
            else
            {
                var existingSchema = db.Schemas.SingleOrDefault(x => x.Name == model.Name);
                if (existingSchema != null)
                    db.Schemas.Remove(existingSchema);
            }

            // Check if we need to use the file or if it's been done for us
            if (model.File != null)
            {
                var sfemResult = await fts.GetSchemaFieldEditModelCollection(model.File);
                if (sfemResult.Succeeded)
                    model.Fields = sfemResult.Result;
                else
                    return Success<SchemaModel>.Error(sfemResult.StatusMessage ?? "Unknown error when translating file.", ErrorCodes.ERR_FILE_PROCESSING_PROBLEM);
            }

            logger.LogInformation($"Creating schema {model.Name}.");

            if (model.AutoDetect)
                model.Fields.ForEach(x => fields.AutoDetectSchemaFieldParameters(x));

            var schemaTextId = db.GetTextId<Schema>(model.Name);

            // Give each of the fields their own Text ID
            model.Fields.ForEach(x => x.TextId = db.GetTextId<SchemaField>($"{schemaTextId}.{x.Name}"));

            // Create the schema database entity and give it a Text ID
            var entity = mapper.Map<Schema>(model);
            entity.TextId = db.GetTextId<Schema>(model.Name);

            // If any of the fields are of the type Set, create the supporting schema that contains the values of the set.
            foreach (var field in entity.SchemaFields.Where(x => x.Type == SchemaFieldType.Set))
            {
                LookupSetModel lsm = new LookupSetModel
                {
                    Name = field.Name,
                    Description = $"Auto-created lookup set to support the schema {model.Name}"
                };
                await lookup.CreateLookupSet(lsm);
            }

            // Make  the first version
            var firstVersion = mapper.Map<SchemaVersion>(entity);
            firstVersion.Version = 1;
            firstVersion.Schema = entity;

            foreach (var field in entity.SchemaFields)
            {
                var versionField = mapper.Map<SchemaFieldVersion>(field);
                versionField.SchemaField = field;
                firstVersion.Fields.Add(versionField);
            }

            var transaction = db.Database.BeginTransaction();

            db.Schemas.Add(entity);
            db.SchemaVersions.Add(firstVersion);

            try
            {
                await db.SaveChangesAsync();
                await iwh.Dispatch(WebHookEventType.SchemaCreated, entity);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to save database changes. No changes were made.");
                transaction.Rollback();
                return Success<SchemaModel>.Error(ex.Message, ErrorCodes.ERR_UNSPECIFIED_ERROR);
            }

            transaction.Commit();

            return new Success<SchemaModel>(mapper.Map<SchemaModel>(entity));

        }

        public async Task<Success<SchemaModel>> GetSchema(string textId)
        {
            if (string.IsNullOrWhiteSpace(textId)) return Success<SchemaModel>.Error("Argument null: textId", ErrorCodes.ERR_ARGUMENT_NULL);

            var schema = await db.Schemas.SingleOrDefaultAsync(x => x.TextId == textId);
            if (schema == null) return Success<SchemaModel>.Error("Schema not found.", ErrorCodes.ERR_NOT_FOUND);

            return new Success<SchemaModel>(mapper.Map<SchemaModel>(schema));

        }

        public async Task<Success> DeleteSchema(string textId)
        {
            if (string.IsNullOrWhiteSpace(textId)) return Success.Error("Argument null: textId", ErrorCodes.ERR_ARGUMENT_NULL);

            var schema = db.Schemas.SingleOrDefault(x => x.TextId == textId);
            if (schema == null) return Success.Error("Schema not found.", ErrorCodes.ERR_NOT_FOUND);

            try
            {
                db.Schemas.Remove(schema);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to save database changes");
                return Success.Error(ex.Message, ErrorCodes.ERR_UNSPECIFIED_ERROR);
            }

            return new Success(true);

        }

        public async Task<Success<SchemaVersionModel>> CreateSchemaVersion(SchemaVersionEditModel model)
        {

            if (model == null) return Success<SchemaVersionModel>.Error("Model cannot be null.", ErrorCodes.ERR_ARGUMENT_NULL);

            if (string.IsNullOrWhiteSpace(model.SchemaTid))
                return Success<SchemaVersionModel>.Error("SchemaTid cannot be null.", ErrorCodes.ERR_ARGUMENT_NULL);

            var schema = db.Schemas.SingleOrDefault(x => x.TextId == model.SchemaTid);
            if (schema == null)
                return Success<SchemaVersionModel>.Error("Schema not found.", ErrorCodes.ERR_NOT_FOUND);

            int newVersion = 1;
            var schemaVersion = db.SchemaVersions.OrderBy(x => x.Version).LastOrDefault();
            if (schemaVersion != null)
                newVersion = schemaVersion.Version + 1;

            logger.LogInformation($"Creating {schema.Name} schema version {newVersion}.");

            var newSchema = new SchemaVersion
            {
                Created = DateTime.Now,
                Name = model.Name ?? schemaVersion?.Name ?? schema.Name,
                Description = schemaVersion?.Description ?? schema.Description,
                SchemaId = schema.Id,
                Version = newVersion
            };

            if (schemaVersion != null)
            {
                foreach (var field in db.SchemaFieldVersions.Where(x => x.SchemaVersionId == schemaVersion.Id))
                {
                    var newField = mapper.Map<SchemaFieldVersion>(field);
                    newField.Id = 0;
                    newField.SchemaVersion = newSchema;
                    newSchema.Fields.Add(newField);
                }
            }
            else
            {
                foreach (var field in db.SchemaFields.Where(x=>x.SchemaId == schema.Id))
                    newSchema.Fields.Add(mapper.Map<SchemaFieldVersion>(field));
            }

            db.SchemaVersions.Add(newSchema);
            await db.SaveChangesAsync();

            return new Success<SchemaVersionModel>(true);

        }

        public async Task<Success<List<SchemaVersionModel>>> GetSchemaVersions(string schemaTid)
        {

            if (string.IsNullOrWhiteSpace(schemaTid))
                return Success<List<SchemaVersionModel>>.Error("SchemaTid cannot be null.", ErrorCodes.ERR_ARGUMENT_NULL);

            var schema = await db.Schemas.SingleOrDefaultAsync(x => x.TextId == schemaTid);
            if (schema == null) return Success<List<SchemaVersionModel>>.Error("Schema not found.", ErrorCodes.ERR_NOT_FOUND);

            var versions = await db.SchemaVersions.Where(x => x.SchemaId == schema.Id).Select(x => mapper.Map<SchemaVersionModel>(x)).ToListAsync();
            return new Success<List<SchemaVersionModel>>(versions);

        }


    }
}
