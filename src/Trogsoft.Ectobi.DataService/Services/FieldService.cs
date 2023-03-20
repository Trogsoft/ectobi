using Microsoft.EntityFrameworkCore;
using Trogsoft.Ectobi.Common;
using Trogsoft.Ectobi.Common.Interfaces;
using Trogsoft.Ectobi.Data;

namespace Trogsoft.Ectobi.DataService.Services
{
    public class FieldService : IFieldService
    {

        private const double UNIQUE_VALUE_MULTIPLIER = 0.98;

        private readonly ILogger<FieldService> logger;
        private readonly EctoDb db;
        private readonly IEctoMapper mapper;

        public FieldService(ILogger<FieldService> logger, EctoDb db, IEctoMapper mapper)
        {
            this.logger = logger;
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<Success<List<SchemaFieldModel>>> GetFields(string schemaTid)
        {
            var fields = await db.SchemaFields.Where(x => x.Schema.TextId == schemaTid).ToListAsync();
            var list = fields.Select(x => mapper.Map<SchemaFieldModel>(x)).ToList();
            return new Success<List<SchemaFieldModel>>(list);
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

            schema.SchemaFields.Add(newField);
            try
            {
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Success<SchemaFieldModel>.Error(ex.Message, ErrorCodes.ERR_UNSPECIFIED_ERROR);
            }

            var returnValue = mapper.Map<SchemaFieldModel>(newField);
            return new Success<SchemaFieldModel>(returnValue);

        }

        public void AutoDetectSchemaFieldParameters(SchemaFieldEditModel x)
        {

            if (x == null) return;

            var nonNullValueCount = x.RawValues.Count(y => y != null);

            // Detect whole numbers
            List<int?> numericValues = new();
            x.RawValues.ForEach(x =>
            {
                if (Int32.TryParse(x, out int val))
                    numericValues.Add(val);
            });

            if (numericValues.Count == nonNullValueCount)
                x.Type = SchemaFieldType.Integer;

            // Detect decimal values
            List<decimal?> decimalValues = new();
            x.RawValues.ForEach(x =>
            {
                if (Decimal.TryParse(x, out decimal val))
                    decimalValues.Add(val);
            });

            if (decimalValues.Count == nonNullValueCount)
                x.Type = SchemaFieldType.Decimal;

            // Detect a set
            var uniqueNonNullValues = x.RawValues.Where(y => !string.IsNullOrWhiteSpace(y)).Distinct();
            var maxUniqueValues = 30;
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
