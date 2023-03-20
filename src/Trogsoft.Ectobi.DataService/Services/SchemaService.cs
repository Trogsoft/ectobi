using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using Trogsoft.Ectobi.Common;
using Trogsoft.Ectobi.Common.Interfaces;
using Trogsoft.Ectobi.Data;

namespace Trogsoft.Ectobi.DataService.Services
{
    public class SchemaService : ISchemaService
    {
        private const double UNIQUE_VALUE_MULTIPLIER = 0.98;
        private readonly ILogger<SchemaService> logger;
        private readonly EctoDb db;
        private readonly IEctoMapper mapper;
        private readonly IFieldService fields;

        public SchemaService(ILogger<SchemaService> logger, EctoDb db, IEctoMapper mapper, IFieldService fields)
        {
            this.logger = logger;
            this.db = db;
            this.mapper = mapper;
            this.fields = fields;
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

            logger.LogInformation($"Creating schema {model.Name}.");

            if (model.AutoDetect)
                model.Fields.ForEach(x => fields.AutoDetectSchemaFieldParameters(x));

            model.Fields.ForEach(x => x.TextId = db.GetTextId<SchemaField>(x.Name));

            var entity = mapper.Map<Schema>(model);
            entity.TextId = db.GetTextId<Schema>(model.Name);

            foreach (var field in entity.SchemaFields.Where(x => x.Type == SchemaFieldType.Set))
            {
                CreateSupportingSchema(entity, field);
            }

            db.Schemas.Add(entity);
            try
            {
                await db.SaveChangesAsync();
            } 
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to save database changes");
                return Success<SchemaModel>.Error(ex.Message, ErrorCodes.ERR_UNSPECIFIED_ERROR);
            }

            return new Success<SchemaModel>(mapper.Map<SchemaModel>(entity));

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

        private void CreateSupportingSchema(Schema entity, SchemaField? field)
        {

            if (field == null) throw new ArgumentNullException(nameof(field));
            if (string.IsNullOrWhiteSpace(field.Name)) throw new ArgumentNullException("field.name");

            var schemaName = field.Name.Trim();
            var tid = db.GetTextId<Schema>($"{entity.TextId}.{schemaName}");

            var nschema = new Schema
            {
                Name = schemaName,
                TextId = tid,
                Description = $"Automatically created for the set of values in {tid}"
            };

            nschema.SchemaFields.Add(new SchemaField
            {
                TextId = db.GetTextId<SchemaField>($"{tid}.NumericId"),
                Name = "Numeric ID",
                Type = SchemaFieldType.Integer,
                Flags = SchemaFieldFlags.NumericID
            });

            nschema.SchemaFields.Add(new SchemaField
            {
                TextId = db.GetTextId<SchemaField>($"{tid}.Value"),
                Name = "Value",
                Type = SchemaFieldType.Text,
                Flags = SchemaFieldFlags.DisplayValue
            });

            db.Schemas.Add(nschema);
            field.ValuesFromSchema = nschema;

        }


    }
}
