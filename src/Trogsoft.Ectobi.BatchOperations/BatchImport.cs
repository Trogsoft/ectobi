using Trogsoft.Ectobi.Common;

namespace Trogsoft.Ectobi.BatchOperations
{
    public class BatchImport : BackgroundJob
    {
        private readonly ImportBatchModel model;

        public BatchImport(BackgroundJobInfo job, ImportBatchModel model) : base(job)
        {
            this.model = model;
        }

        public Success Execute()
        {
            return new Success();
        }

    }
}