using Microsoft.EntityFrameworkCore;
using Trogsoft.Ectobi.Common;
using Trogsoft.Ectobi.Common.Interfaces;
using Trogsoft.Ectobi.Data;
using Trogsoft.Ectobi.DataService.Data;
using Trogsoft.Ectobi.DataService.Interfaces;
using Trogsoft.Ectobi.DataService.Validation;

namespace Trogsoft.Ectobi.DataService.Services;

public class SchemaService : ISchemaService
{
    private readonly ILogger<SchemaService> logger;
    private readonly EctoDb db;
    private readonly IEctoMapper mapper;
    private readonly IInternalFieldService fields;
    private readonly ILookupService lookup;
    private readonly IWebHookService iwh;
    private readonly IFileTranslatorService fts;
    private readonly IEctoData data;

    public SchemaService(ILogger<SchemaService> logger, EctoDb db, IEctoMapper mapper, IInternalFieldService fields,
        ILookupService lookup, IWebHookService iwh, IFileTranslatorService fts, IEctoData data)
    {
        this.logger = logger;
        this.db = db;
        this.mapper = mapper;
        this.fields = fields;
        this.lookup = lookup;
        this.iwh = iwh;
        this.fts = fts;
        this.data = data;
    }

    public async Task<Success<List<SchemaModel>>> GetSchemas(bool includeDetails = false)
    {
        try
        {
            var schemas = (await data.Schema.GetSchemas()).ToList();
            return new Success<List<SchemaModel>>(schemas);
        }
        catch (Exception ex)
        {
            return Success<List<SchemaModel>>.Error(ex.Message, ErrorCodes.ERR_UNSPECIFIED_ERROR);
        }
    }

    public async Task<Success<SchemaModel>> CreateSchema(SchemaEditModel model)
    {

        var validator = EctoModelValidator.CreateValidator<SchemaEditModel>()
            .WithModel(model)
            .Property(x => x.Id).MustEqual(0)
            .Property(x => x.Name).NotNullOrWhiteSpace();

        if (!validator.Validate()) return validator.GetResult<SchemaModel>();

        // Check if we need to use the file or if it's been done for us
        if (model.File != null && model.File.Filename != null && model.File.Data != null)
        {

            var sfemResult = await fts.GetSchemaFieldEditModelCollection(model.File);
            if (sfemResult.Succeeded && sfemResult.Result != null)
                model.Fields = sfemResult.Result;
            else
                return Success<SchemaModel>.Error(sfemResult.StatusMessage ?? "Unknown error when translating file.", ErrorCodes.ERR_FILE_PROCESSING_PROBLEM);

        }

        data.BeginTransaction();

        SchemaModel? createdSchema;
        try
        {
            createdSchema = await data.Schema.CreateSchema(model);
        }
        catch (Exception ex)
        {
            data.RollbackTransaction();
            return Success<SchemaModel>.Error(ex.Message, ErrorCodes.ERR_UNSPECIFIED_ERROR);
        }

        if (model.AutoDetect)
            model.Fields.ForEach(x => fields.AutoDetectSchemaFieldParameters(x));

        // Create fields

        foreach (var field in model.Fields)
        {

            string? lookupSetTid = null;
            if (field.Type == SchemaFieldType.Set)
            {
                var lsm = new LookupSetModel
                {
                    Name = field.Name,
                    Description = $"Auto-created lookup set to support the schema {model.Name}",
                    Values = field.RawValues.Distinct().Select((x, i) => new LookupSetValueModel { Name = x, Value = i + 1 }).ToList(),
                    SchemaTid = createdSchema.TextId
                };
                var existingLookupSet = await data.Lookup.FindMatchingLookupSets(lsm);
                if (existingLookupSet.Any())
                    lookupSetTid = existingLookupSet.First().TextId;
                else
                {
                    var newLookupSet = await data.Lookup.CreateLookupSet(lsm);
                    lookupSetTid = newLookupSet.TextId;
                }
            }

            field.SchemaTid = createdSchema.TextId;
            field.LookupTid = lookupSetTid;
            SchemaField? newField;
            try
            {
                newField = await data.Field.CreateRootField(field);
            }
            catch (Exception ex)
            {
                data.RollbackTransaction();
                return Success<SchemaModel>.Error(ex.Message, ErrorCodes.ERR_UNSPECIFIED_ERROR);
            }

            // Create the first version of the field
            var fieldVersionModel = mapper.Map<SchemaField, SchemaFieldEditModel>(newField);
            fieldVersionModel.LookupTid = lookupSetTid;
            fieldVersionModel.Version = 1;
            fieldVersionModel.SchemaTid = createdSchema.TextId;

            try
            {
                await data.Field.CreateVersionField(fieldVersionModel);
            }
            catch (Exception ex)
            {
                data.RollbackTransaction();
                return Success<SchemaModel>.Error(ex.Message, ErrorCodes.ERR_UNSPECIFIED_ERROR);
            }

        }

        data.CommitTransaction();

        var commitedSchema = await data.Schema.GetSchema(createdSchema.TextId!, true);
        return new Success<SchemaModel>(mapper.Map<SchemaModel>(commitedSchema));
    }

    public async Task<Success<SchemaModel>> GetSchema(string textId)
    {

        if (string.IsNullOrWhiteSpace(textId)) return Success<SchemaModel>.Error("Argument null: textId", ErrorCodes.ERR_ARGUMENT_NULL);

        try
        {
            var schema = await data.Schema.GetSchema(textId);
            return new Success<SchemaModel>(schema);
        }
        catch (SchemaNotFoundException)
        {
            return Success<SchemaModel>.Error("Schema not found.", ErrorCodes.ERR_NOT_FOUND);
        }
        catch (Exception ex)
        {
            return Success<SchemaModel>.Error(ex.Message, ErrorCodes.ERR_UNSPECIFIED_ERROR);
        }

    }

    public async Task<Success> DeleteSchema(string textId)
    {
        if (string.IsNullOrWhiteSpace(textId)) return Success.Error("Argument null: textId", ErrorCodes.ERR_ARGUMENT_NULL);

        try
        {
            await data.Schema.DeleteSchema(textId);
            return new Success(true);
        }
        catch (SchemaNotFoundException)
        {
            return Success.Error("Schema not found.", ErrorCodes.ERR_NOT_FOUND);
        }
        catch (Exception ex)
        {
            return Success.Error(ex.Message, ErrorCodes.ERR_UNSPECIFIED_ERROR);
        }

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
            foreach (var field in db.SchemaFields.Where(x => x.SchemaId == schema.Id))
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
