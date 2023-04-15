using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Reflection;
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

        public async Task<Success<List<EctoModelDefinition>>> GetModelDefinitions()
        {

            List<EctoModelDefinition> models = new List<EctoModelDefinition>();
            foreach (var type in mm.GetModelTypes())
            {
                var model = getModelDefinitionFromType(type);
                if (model.Succeeded)
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
                };

                var ptype = SchemaFieldType.Text;
                if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(int?)) ptype = SchemaFieldType.Integer;
                if (prop.PropertyType == typeof(long) || prop.PropertyType == typeof(long?)) ptype = SchemaFieldType.Integer;
                if (prop.PropertyType == typeof(bool) || prop.PropertyType == typeof(bool?)) ptype = SchemaFieldType.Boolean;
                if (prop.PropertyType == typeof(DateTime) | prop.PropertyType == typeof(DateTime?)) ptype = SchemaFieldType.DateTime;
                if (prop.PropertyType == typeof(decimal) | prop.PropertyType == typeof(decimal?)) ptype = SchemaFieldType.Decimal;
                if (prop.PropertyType == typeof(double) | prop.PropertyType == typeof(double?)) ptype = SchemaFieldType.Decimal;
                if (prop.PropertyType == typeof(float) | prop.PropertyType == typeof(float?)) ptype = SchemaFieldType.Decimal;

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

            if (model == null) return Success<List<SchemaFieldModel>>.Error("Model is null.", ErrorCodes.ERR_ARGUMENT_NULL);

            if (string.IsNullOrWhiteSpace(model.SchemaTid))
                return Success<List<SchemaFieldModel>>.Error("SchemaTid was not specified.", ErrorCodes.ERR_ARGUMENT_NULL);

            if (string.IsNullOrWhiteSpace(model.ModelTid))
                return Success<List<SchemaFieldModel>>.Error("ModelTid was not specified.", ErrorCodes.ERR_ARGUMENT_NULL);

            var schema = await db.Schemas.SingleOrDefaultAsync(x => x.TextId == model.SchemaTid);
            if (schema == null) return Success<List<SchemaFieldModel>>.Error("Schema not found.", ErrorCodes.ERR_NOT_FOUND);

            var dmodel = await db.Models.SingleOrDefaultAsync(x => x.TextId == model.ModelTid);
            if (dmodel == null) return Success<List<SchemaFieldModel>>.Error("Model not found.", ErrorCodes.ERR_NOT_FOUND);

            var imodel = this.GetModelDefinition(model.ModelTid);
            if (!imodel.Succeeded | imodel.Result == null)
                return Success<List<SchemaFieldModel>>.Error(imodel.StatusMessage ?? "Model not found.", imodel.ErrorCode);

            var fields = imodel.Result!.Properties.IntersectBy(model.Properties, x => x.TextId).ToList();

            foreach (var field in fields)
            {
                var existingField = await db.SchemaFields.SingleOrDefaultAsync(x => x.Name == field.Name);

                var sff = SchemaFieldFlags.None;
                if (field.Flags.HasFlag(EctoModelPropertyFlags.PersonallyIdentifiableInformation)) sff |= SchemaFieldFlags.PersonallyIdentifiableInformation;

                var type = SchemaFieldType.Text;
                if (field.Type == typeof(int) || field.Type == typeof(int?)) type = SchemaFieldType.Integer;
                if (field.Type == typeof(long) || field.Type == typeof(long?)) type = SchemaFieldType.Integer;
                if (field.Type == typeof(bool) || field.Type == typeof(bool?)) type = SchemaFieldType.Boolean;
                if (field.Type == typeof(DateTime) | field.Type == typeof(DateTime?)) type = SchemaFieldType.DateTime;
                if (field.Type == typeof(decimal) | field.Type == typeof(decimal?)) type = SchemaFieldType.Decimal;
                if (field.Type == typeof(double) | field.Type == typeof(double?)) type = SchemaFieldType.Decimal;
                if (field.Type == typeof(float) | field.Type == typeof(float?)) type = SchemaFieldType.Decimal;

                await fieldService.CreateField(model.SchemaTid, new SchemaFieldEditModel
                {
                    Name = field.Name,
                    Description = field.Description,
                    ModelTid = model.ModelTid,
                    ModelField = field.TextId,  // start here
                    Flags = sff,
                    Type = type
                });
            }

            return await fieldService.GetFields(model.SchemaTid);

        }

    }
}
