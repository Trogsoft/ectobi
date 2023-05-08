using Trogsoft.Ectobi.Common;

namespace Trogsoft.Ectobi.DataService.Data
{
    public interface ISchemaStore
    {
        Task<SchemaModel> CreateSchema(SchemaEditModel model);
        Task<SchemaVersionModel> CreateSchemaVersion(SchemaVersionEditModel model);
        Task DeleteSchema(string schemaName);
        Task<long> GetLatestVersionEntityId(string schema);
        Task<SchemaModel> GetSchema(string schemaName, bool includeDetail = false);
        Task<long?> GetSchemaId(string schemaName);
        Task<IEnumerable<SchemaModel>> GetSchemas();
        Task<SchemaVersionModel> GetSchemaVersion(string schemaName, int version = 0);
        Task<bool> SchemaExists(string schemaName);
    }
}