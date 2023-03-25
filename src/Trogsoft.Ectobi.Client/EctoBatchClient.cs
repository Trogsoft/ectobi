using Trogsoft.Ectobi.Common;

namespace Trogsoft.Ectobi.Client
{
    public class EctoBatchClient : EctoClientBase
    {
        public EctoBatchClient(HttpClient client) : base(client)
        {
        }

        public async Task<Success<BackgroundTaskInfo>> ImportBatch(ImportBatchModel import)
        {
            var result = await PostJsonAsync<BackgroundTaskInfo>("api/batch", import);
            return result;
        }

    }
}