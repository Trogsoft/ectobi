using Microsoft.EntityFrameworkCore;
using Trogsoft.Ectobi.Common;
using Trogsoft.Ectobi.Common.Interfaces;
using Trogsoft.Ectobi.Data;

namespace Trogsoft.Ectobi.DataService.Services
{
    public class ModelService : IModelService
    {
        private readonly ModuleManager mm;
        private readonly EctoDb db;
        private readonly IFieldService fieldService;

        public ModelService(ModuleManager mm, EctoDb db, IFieldService fieldService)
        {
            this.mm = mm;
            this.db = db;
            this.fieldService = fieldService;
        }

        public async Task<Success<List<EctoModelDefinition>>> GetModelDefinitions() => mm.GetModelDefinitions();

        public async Task<Success<List<SchemaFieldModel>>> ConfigureModel(ModelConfigurationModel model)
        {

            if (model == null) return Success<List<SchemaFieldModel>>.Error("Model is null.", ErrorCodes.ERR_ARGUMENT_NULL);

            if (string.IsNullOrWhiteSpace(model.SchemaTid))
                return Success<List<SchemaFieldModel>>.Error("SchemaTid was not specified.", ErrorCodes.ERR_ARGUMENT_NULL);

            if (string.IsNullOrWhiteSpace(model.ModelTid))
                return Success<List<SchemaFieldModel>>.Error("ModelTid was not specified.", ErrorCodes.ERR_ARGUMENT_NULL);

            var schema = await db.Schemas.SingleOrDefaultAsync(x => x.TextId == model.SchemaTid);
            if (schema == null) return Success<List<SchemaFieldModel>>.Error("Schema not found.", ErrorCodes.ERR_NOT_FOUND);

            var dmodel = await db.Models.SingleOrDefaultAsync(x => x.TextId == model.ModelTid);
            if (dmodel == null) return Success<List<SchemaFieldModel>>.Error("Model not found.", ErrorCodes.ERR_NOT_FOUND);

            var imodel = mm.GetModelDefinition(model.ModelTid);
            if (!imodel.Succeeded | imodel.Result == null)
                return Success<List<SchemaFieldModel>>.Error(imodel.StatusMessage ?? "Model not found.", imodel.ErrorCode);

            var fields = imodel.Result!.Properties.Where(x => model.Properties.Contains(x.Name)).ToList();

            foreach (var field in fields)
            {
                var existingField = await db.SchemaFields.SingleOrDefaultAsync(x => x.Name == field.Name);

                await fieldService.CreateField(model.SchemaTid, new SchemaFieldEditModel
                {
                    Name = field.Name,
                    Description = field.Description,
                    ModelTid = model.ModelTid
                });
            }

            return await fieldService.GetFields(model.SchemaTid);

        }

    }
}
