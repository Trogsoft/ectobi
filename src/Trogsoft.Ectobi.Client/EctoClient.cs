using System.Net.Http.Headers;

namespace Trogsoft.Ectobi.Client
{
    public class EctoClient 
    {
        private readonly HttpClient client;

        public EctoClient() : this(new())
        {
        }

        public EctoClient(HttpClient client)
        {
            this.client = client;
            initialize();
        }

        private void initialize()
        {
            this.Schema = new EctoSchemaClient(client);
            this.Fields = new EctoFieldClient(client);
            this.Batches = new EctoBatchClient(client);
            this.SchemaVersions = new EctoSchemaVersionClient(client);
        }

        public EctoSchemaClient Schema { get; private set; }
        public EctoFieldClient Fields { get; private set; }
        public EctoBatchClient Batches { get; private set; }
        public EctoSchemaVersionClient SchemaVersions { get; private set; }

    }
}