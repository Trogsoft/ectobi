using Newtonsoft.Json;
using System.Net.Http.Json;
using Trogsoft.Ectobi.Common;

namespace Trogsoft.Ectobi.Client
{
    public class EctoSchemaClient : EctoClientBase
    {
        public EctoSchemaClient(HttpClient client) : base(client)
        {
        }

        public async Task<Success<SchemaModel>> CreateSchemaAsync(SchemaEditModel schema)
        {
            if (schema == null) return Success<SchemaModel>.Error("Schema cannot be null.");
            if (schema.Id > 0) return Success<SchemaModel>.Error("Schema already exists.");
            var result = await PostJsonAsync<SchemaModel>("api/schema", schema);
            return result;
        }

        public async Task<Success> DeleteSchemaSync(string schemaId)
        {
            if (string.IsNullOrWhiteSpace(schemaId)) return Success.Error("SchemaId cannot be null.", ErrorCodes.ERR_ARGUMENT_NULL);
            var result = await DeleteAsync("api/schema/" + schemaId);
            return result;
        }

        public async Task<Success<List<SchemaModel>>> GetSchemas(bool includeDetail)
        {
            var result = await GetAsync<List<SchemaModel>>("api/schema");
            return result;
        }

    }
}