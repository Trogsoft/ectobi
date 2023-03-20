using Hangfire.Server;
using Trogsoft.Ectobi.Common;
using Trogsoft.Ectobi.Common.Interfaces;
using Trogsoft.Ectobi.Data;

namespace Trogsoft.Ectobi.DataService.Services
{
    public class BatchService : IBatchService
    {
        private readonly ILogger<BatchService> logger;
        private readonly EctoDb db;
        private readonly IEctoMapper mapper;
        private readonly IBackgroundTaskCoordinator bg;

        public BatchService(ILogger<BatchService> logger, EctoDb db, IEctoMapper mapper, IBackgroundTaskCoordinator bg) {
            this.logger = logger;
            this.db = db;
            this.mapper = mapper;
            this.bg = bg;
        }

        public async Task<Success<BackgroundTaskInfo>> ImportBatch(ImportBatchModel model)
        {
            BackgroundTaskInfo job = new BackgroundTaskInfo();
            bg.Enqueue<IBatchService>(x => x.BackgroundImportBatch(job, model));
            return new Success<BackgroundTaskInfo>(job);
        }

        public Success BackgroundImportBatch(BackgroundTaskInfo job, ImportBatchModel model)
        {
            throw new NotImplementedException();
        }

    }
}
