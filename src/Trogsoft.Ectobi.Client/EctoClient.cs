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
        }

        public EctoSchemaClient Schema { get; private set; }
        public EctoFieldClient Fields { get; private set; }

    }
}