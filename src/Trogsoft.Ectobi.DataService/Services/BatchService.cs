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
        private readonly IBackgroundJobCoordinator bg;

        public BatchService(ILogger<BatchService> logger, EctoDb db, IEctoMapper mapper, IBackgroundJobCoordinator bg) {
            this.logger = logger;
            this.db = db;
            this.mapper = mapper;
            this.bg = bg;
        }

        public async Task<Success<BackgroundJobInfo>> ImportBatch(ImportBatchModel model)
        {
            BackgroundJobInfo job = new BackgroundJobInfo();
            bg.Enqueue<IBatchService>(x => x.BackgroundImportBatch(job, model));
            return new Success<BackgroundJobInfo>(job);
        }

        public Success BackgroundImportBatch(BackgroundJobInfo job, ImportBatchModel model)
        {
            throw new NotImplementedException();
        }

    }
}
