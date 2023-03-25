using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trogsoft.Ectobi.Common;

namespace Trogsoft.Ectobi.Client
{
    public class EctoSchemaVersionClient : EctoClientBase
    {
        public EctoSchemaVersionClient(HttpClient client) : base(client)
        {
        }

        public async Task<Success<List<SchemaVersionModel>>> GetSchemaVersions(string schemaTid)
        {
            var result = await GetAsync<List<SchemaVersionModel>>($"api/schema/{schemaTid}/versions");
            return result;
        }

        public async Task<Success<SchemaVersionModel>> CreateSchemaVersion(SchemaVersionEditModel model)
        {
            var result = await PostJsonAsync<SchemaVersionModel>($"api/schema/{model.SchemaTid}/versions", model);
            return result;
        }

    }
}
