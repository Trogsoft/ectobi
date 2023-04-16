using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Reflection;
using Trogsoft.Ectobi.Common;
using Trogsoft.Ectobi.Common.Interfaces;
using Trogsoft.Ectobi.Data;
using Trogsoft.Ectobi.DataService.Validation;

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

        public async Task<Success<List<EctoModelDefinition>>> GetModelDefinitions()
        {

            List<EctoModelDefinition> models = new List<EctoModelDefinition>();
            foreach (var type in mm.GetModelTypes())
            {
                var model = getModelDefinitionFromType(type);
                if (model.Succeeded && model.Result != null)
                    models.Add(model.Result);
            }

            return new Success<List<EctoModelDefinition>>(models);

        }

        private Success<EctoModelDefinition> GetModelDefinition(string modelName)
        {

            var type = mm.GetModelTypes().SingleOrDefault(x => x.Name.Equals(modelName, StringComparison.CurrentCultureIgnoreCase));
            if (type == null) return Success<EctoModelDefinition>.Error("Type not found.", ErrorCodes.ERR_NOT_FOUND);

            return getModelDefinitionFromType(type);

        }

        private Success<EctoModelDefinition> getModelDefinitionFromType(Type type)
        {
            var attr = type.GetCustomAttribute<EctoModelAttribute>();
            if (attr == null) return Success<EctoModelDefinition>.Error($"Type {type.Name} is missing a [EctoModel] attribute.", ErrorCodes.ERR_NOT_FOUND);

            var moduleDefinition = new EctoModelDefinition
            {
                TextId = type.Name,
                Name = attr.Title,
                Description = attr.Description
            };

            foreach (var prop in type.GetProperties())
            {
                var ep = new EctoModelProperty
                {
                    Name = prop.Name,
                    TextId = prop.Name
                };

                var ptype = SchemaFieldType.Text;
                if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(int?)) ptype = SchemaFieldType.Integer;
                if (prop.PropertyType == typeof(long) || prop.PropertyType == typeof(long?)) ptype = SchemaFieldType.Integer;
                if (prop.PropertyType == typeof(bool) || prop.PropertyType == typeof(bool?)) ptype = SchemaFieldType.Boolean;
                if (prop.PropertyType == typeof(DateTime) | prop.PropertyType == typeof(DateTime?)) ptype = SchemaFieldType.DateTime;
                if (prop.PropertyType == typeof(decimal) | prop.PropertyType == typeof(decimal?)) ptype = SchemaFieldType.Decimal;
                if (prop.PropertyType == typeof(double) | prop.PropertyType == typeof(double?)) ptype = SchemaFieldType.Decimal;
                if (prop.PropertyType == typeof(float) | prop.PropertyType == typeof(float?)) ptype = SchemaFieldType.Decimal;

                ep.Type = ptype;

                var displayNameAttr = prop.GetCustomAttribute<DisplayNameAttribute>();
                if (displayNameAttr != null)
                    ep.Name = displayNameAttr.DisplayName;

                var descriptionAttr = prop.GetCustomAttribute<DescriptionAttribute>();
                if (descriptionAttr != null)
                    ep.Description = descriptionAttr.Description;

                var flagsAttr = prop.GetCustomAttribute<ModelFlagsAttribute>();
                if (flagsAttr != null)
                    ep.Flags = flagsAttr.Flags;

                moduleDefinition.Properties.Add(ep);

            }

            return new Success<EctoModelDefinition>(moduleDefinition);
        }

        public async Task<Success<List<SchemaFieldModel>>> ConfigureModel(ModelConfigurationModel model)
        {

            var validator = EctoModelValidator.CreateValidator<ModelConfigurationModel>(db)
                .WithModel(model)
                .Property(x => x.SchemaTid).NotNullOrWhiteSpace()
                .Property(x => x.ModelName).NotNullOrWhiteSpace()
                .Entity<Schema>(x => x.SchemaTid!).MustExist()
                .Entity<Model>(x => x.ModelName!).MustExist();

            if (!validator.Validate()) return validator.GetResult<List<SchemaFieldModel>>();

            //if (!validator.Validate()) return validator.GetReturnValue<List<SchemaFieldModel>>();

            //if (model == null) return Success<List<SchemaFieldModel>>.Error("Model is null.", ErrorCodes.ERR_ARGUMENT_NULL);

            //if (string.IsNullOrWhiteSpace(model.SchemaTid))
            //    return Success<List<SchemaFieldModel>>.Error("SchemaTid was not specified.", ErrorCodes.ERR_ARGUMENT_NULL);

            //if (string.IsNullOrWhiteSpace(model.ModelName))
            //    return Success<List<SchemaFieldModel>>.Error("ModelTid was not specified.", ErrorCodes.ERR_ARGUMENT_NULL);

            //var schema = await db.Schemas.SingleOrDefaultAsync(x => x.TextId == model.SchemaTid);
            //if (schema == null) return Success<List<SchemaFieldModel>>.Error("Schema not found.", ErrorCodes.ERR_NOT_FOUND);

            //var dmodel = await db.Models.SingleOrDefaultAsync(x => x.TextId == model.ModelName);
            //if (dmodel == null) return Success<List<SchemaFieldModel>>.Error("Model not found.", ErrorCodes.ERR_NOT_FOUND);

            var imodel = GetModelDefinition(model.ModelName);
            if (!imodel.Succeeded | imodel.Result == null)
                return Success<List<SchemaFieldModel>>.Error(imodel.StatusMessage ?? "Model not found.", imodel.ErrorCode);

            var fields = imodel.Result!.Properties.IntersectBy(model.Properties, x => x.TextId).ToList();

            foreach (var field in fields)
            {

                // If it already exists, that's fine and we can move to the next one.
                var existingField = await db.SchemaFields.SingleOrDefaultAsync(x => x.Name == field.Name);
                if (existingField != null) continue;

                var sff = SchemaFieldFlags.None;
                if (field.Flags.HasFlag(EctoModelPropertyFlags.PersonallyIdentifiableInformation)) sff |= SchemaFieldFlags.PersonallyIdentifiableInformation;

                await fieldService.CreateField(model.SchemaTid, new SchemaFieldEditModel
                {
                    Name = field.Name,
                    Description = field.Description,
                    ModelName = model.ModelName,
                    ModelField = field.TextId,  // start here
                    Flags = sff,
                    Type = field.Type
                });
            }

            return await fieldService.GetFields(model.SchemaTid);

        }

    }
}
