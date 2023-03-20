using Trogsoft.Ectobi.Common;

namespace Trogsoft.Ectobi.Client
{
    public class EctoFieldClient : EctoClientBase
    {
        public EctoFieldClient(HttpClient client) : base(client)
        {
        }

        public async Task<Success<List<SchemaFieldModel>>> GetFields(string schemaTid)
        {
            var result = await GetAsync<List<SchemaFieldModel>>($"api/field/{schemaTid}");
            return result;
        }

        public async Task<Success<SchemaFieldModel>> CreateField(string schemaTid, SchemaFieldEditModel model)
        {
            var result = await PostJsonAsync<SchemaFieldModel>($"api/field/{schemaTid}", model);
            return result;
        }

    }
}