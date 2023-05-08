using Microsoft.EntityFrameworkCore;
using Trogsoft.Ectobi.Common;
using Trogsoft.Ectobi.Data;

namespace Trogsoft.Ectobi.DataService.Data
{
    internal class SchemaStore : ISchemaStore
    {
        private readonly EctoData data;

        public SchemaStore(EctoData data)
        {
            this.data = data;
        }

        public async Task<long?> GetSchemaId(string schemaName) => (await data.Store.Schemas.SingleOrDefaultAsync(x => x.TextId == schemaName))?.Id;

        public async Task<bool> SchemaExists(string schemaName) => await data.Store.Schemas.AnyAsync(x => x.TextId == schemaName);

        public async Task<SchemaModel> GetSchema(string schemaName, bool includeDetail = false)
        {
            if (!(await SchemaExists(schemaName))) throw new SchemaNotFoundException();
            var query = data.Store.Schemas.AsQueryable();
            if (includeDetail) query = query.Include(x => x.SchemaFields);
            var schema = (await query.SingleOrDefaultAsync(x => x.TextId == schemaName))!;
            return data.Mapper.Map<Schema, SchemaModel>(schema);
        }

        public async Task<IEnumerable<SchemaModel>> GetSchemas()
            => await data.Store.Schemas.Select(x => data.Mapper.Map<Schema, SchemaModel>(x)).ToListAsync();

        public async Task DeleteSchema(string schemaName)
        {
            if (!await SchemaExists(schemaName)) throw new SchemaNotFoundException();
            var schema = (await data.Store.Schemas.SingleOrDefaultAsync(x => x.TextId == schemaName))!;
            data.Store.Batches.RemoveRange(await data.Store.Batches.Where(x => x.SchemaVersion.SchemaId == schema.Id).ToListAsync());
            (await data.Store.LookupSets.Where(x => x.SchemaId == schema.Id).ToListAsync()).ForEach(ls => data.Lookup.DeleteLookupSet(ls.TextId!));
            data.Store.Schemas.Remove(schema);
            await data.Store.SaveChangesAsync();
        }

        /// <summary>
        /// Creates a schema and a first schema version. No fields are created as part of this method.
        /// </summary>
        /// <param name="model">The schema to create</param>
        /// <returns>The new schema</returns>
        public async Task<SchemaModel> CreateSchema(SchemaEditModel model)
        {
            model.TextId = data.Store.GetTextId<Schema>(model.Name ?? "Untitled Schema");
            var schema = data.Mapper.Map<SchemaEditModel, Schema>(model);

            data.Store.Schemas.Add(schema);
            await data.Store.SaveChangesAsync();

            var schemaVersion = data.Mapper.Map<Schema, SchemaVersionEditModel>(schema);
            schemaVersion.SchemaTid = schema.TextId!;
            await this.CreateSchemaVersion(schemaVersion);

            return data.Mapper.Map<Schema, SchemaModel>(schema);
        }

        public async Task<SchemaVersionModel> GetSchemaVersion(string schemaName, int version = 0)
        {
            if (!await SchemaExists(schemaName)) throw new SchemaNotFoundException();
            var qualifyingSchemas = data.Store.SchemaVersions.Where(x => x.Schema.TextId == schemaName).OrderByDescending(x => x.Version);
            SchemaVersion? schemaVersion = null;
            if (version == 0)
                schemaVersion = qualifyingSchemas.FirstOrDefault();
            else
                schemaVersion = qualifyingSchemas.SingleOrDefault(x => x.Version == version);

            if (schemaVersion == null) throw new SchemaVersionNotFoundException();
            return data.Mapper.Map<SchemaVersion, SchemaVersionModel>(schemaVersion);
        }

        public async Task<SchemaVersionModel> CreateSchemaVersion(SchemaVersionEditModel model)
        {
            var schemaVersion = data.Mapper.Map<SchemaVersionEditModel, SchemaVersion>(model);
            var schemaId = await GetSchemaId(model.SchemaTid);
            if (schemaId == null) throw new SchemaNotFoundException();

            if (schemaVersion.Version == 0) schemaVersion.Version = 1;
            schemaVersion.SchemaId = schemaId.Value;
            schemaVersion.Created = DateTime.Now;
            data.Store.SchemaVersions.Add(schemaVersion);
            await data.Store.SaveChangesAsync();
            return data.Mapper.Map<SchemaVersion, SchemaVersionModel>(schemaVersion);
        }

        public async Task<long> GetLatestVersionEntityId(string schema)
        {
            if (!(await SchemaExists(schema))) throw new SchemaNotFoundException();
            var latestSchema = data.Store.SchemaVersions.Where(x => x.Schema.TextId == schema).OrderByDescending(x => x.Version).FirstOrDefault();
            if (latestSchema == null) throw new SchemaVersionNotFoundException();
            return latestSchema.Version;
        }
    }
}