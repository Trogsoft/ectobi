using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using Trogsoft.Ectobi.Common;
using Trogsoft.Ectobi.Data;

namespace Trogsoft.Ectobi.DataService.Data
{
    internal class FieldStore : IFieldStore
    {
        private EctoData data;

        public FieldStore(EctoData data)
        {
            this.data = data;
        }

        public async Task<SchemaField> CreateRootField(SchemaFieldEditModel fieldDef)
        {
            fieldDef.TextId = data.Store.GetTextId<SchemaField>(fieldDef.Name ?? "Untitled Field");
            var schemaField = data.Mapper.Map<SchemaFieldModel, SchemaField>(fieldDef);
            var schemaId = await data.Schema.GetSchemaId(fieldDef.SchemaTid);

            if (schemaId.HasValue)
                schemaField.SchemaId = schemaId.Value;
            else
                throw new SchemaNotFoundException();

            data.Store.SchemaFields.Add(schemaField);
            await data.Store.SaveChangesAsync();

            var webHookModel = data.Mapper.Map<SchemaField, SchemaFieldModel>(schemaField);
            await data.WebHooks.Dispatch(WebHookEventType.FieldCreated, new WebHookEventModel<SchemaFieldModel>(webHookModel));

            return schemaField;
        }

        public async Task<SchemaFieldModel> CreateVersionField(SchemaFieldEditModel model)
        {
            var schemaVersion = await data.Schema.GetSchemaVersion(model.SchemaTid, model.Version);
            var rootField = await this.GetRootField(model.SchemaTid, model.TextId);

            var schemaField = data.Mapper.Map<SchemaFieldEditModel, SchemaFieldVersion>(model);
            schemaField.SchemaVersionId = schemaVersion.Id;
            schemaField.SchemaFieldId = rootField.Id;

            if (!string.IsNullOrWhiteSpace(model.LookupTid))
                schemaField.LookupSetId = await data.Lookup.GetLookupSetId(model.LookupTid);

            data.Store.SchemaFieldVersions.Add(schemaField);
            await data.Store.SaveChangesAsync();

            var webHookModel = data.Mapper.Map<SchemaFieldVersion, SchemaFieldModel>(schemaField);
            await data.WebHooks.Dispatch(WebHookEventType.FieldCreated, new WebHookEventModel<SchemaFieldModel>(webHookModel));

            return data.Mapper.Map<SchemaFieldVersion, SchemaFieldModel>(schemaField);
        }

        public async Task<bool> OtherFieldVersionsExist(string schemaTid, string fieldTid, int version)
        {
            return await data.Store.SchemaFieldVersions.AnyAsync(x =>
                x.SchemaVersion.Version != version
                && x.SchemaField.TextId == fieldTid
                && x.SchemaVersion.Schema.TextId == schemaTid);
        }

        public async Task<bool> DeleteField(string schemaTid, string fieldTid, int version = 0)
        {
            var schemaVersion = await data.Schema.GetSchemaVersion(schemaTid, version);
            var field = await data.Store.SchemaFieldVersions.SingleOrDefaultAsync(x => x.SchemaVersionId == schemaVersion.Id && x.SchemaField.TextId == fieldTid);
            if (field == null) throw new FieldNotFoundException();

            await data.Data.DeleteAllFieldValues(schemaTid, fieldTid, version);
            data.Store.SchemaFieldVersions.Remove(field);
            await data.Store.SaveChangesAsync();

            if (!(await OtherFieldVersionsExist(schemaTid, fieldTid, schemaVersion.Version)))
                await DeleteRootField(schemaTid, fieldTid);

            await data.WebHooks.Dispatch(WebHookEventType.FieldDeleted, new WebHookFieldEvent(schemaTid, version, fieldTid));
            return true;
        }

        public async Task<bool> DeleteRootField(string schemaTid, string fieldTid)
        {
            var field = await data.Store.SchemaFields.SingleOrDefaultAsync(x => x.Schema.TextId == schemaTid && x.TextId == fieldTid);
            if (field == null) throw new FieldNotFoundException();

            data.Store.SchemaFields.Remove(field);
            await data.Store.SaveChangesAsync();
            await data.WebHooks.Dispatch(WebHookEventType.FieldDeleted, new WebHookFieldEvent(schemaTid, fieldTid));
            return true;
        }

        public async Task<SchemaFieldModel> GetRootField(string schemaTid, string fieldTid)
        {
            if (!(await data.Schema.SchemaExists(schemaTid))) throw new SchemaNotFoundException();
            var schema = await data.Schema.GetSchema(schemaTid);
            var field = await data.Store.SchemaFields.SingleOrDefaultAsync(x => x.TextId == fieldTid && x.Schema.Id == schema.Id);
            if (field != null)
                return data.Mapper.Map<SchemaField, SchemaFieldModel>(field);
            throw new FieldNotFoundException();
        }

        public async Task SetFieldPopulator(SchemaFieldModel versionField, string populator)
        {
            var dbField = await data.Store.SchemaFieldVersions.SingleOrDefaultAsync(x => x.Id == versionField.Id);
            if (dbField == null) throw new FieldNotFoundException();

            dbField.PopulatorId = data.ModuleManager.GetPopulatorDatabaseId(populator);
            await data.Store.SaveChangesAsync();
        }

        public async Task SetFieldModel(SchemaFieldModel versionField, string modelName, string modelField)
        {
            var dbField = await data.Store.SchemaFieldVersions.SingleOrDefaultAsync(x => x.Id == versionField.Id);
            if (dbField == null) throw new FieldNotFoundException();

            var dbModel = await data.Store.Models.SingleOrDefaultAsync(x => x.TextId == modelName);
            if (dbModel == null) throw new ModelNotFoundException();

            dbField.ModelId = dbModel.Id;
            dbField.ModelField = modelField;

            await data.Store.SaveChangesAsync();
        }
    }
}