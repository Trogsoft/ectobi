using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trogsoft.Ectobi.Common.Interfaces
{
    public interface IBatchService
    {
        Success BackgroundImportBatch(BackgroundTaskInfo job, ImportBatchModel model);
        Task<Success<BatchModel>> CreateEmptyBatch(BatchModel model);
        Success ExecutePopulators(long id);
        Task<Success<List<BatchModel>>> GetBatches(string schemaTid);
        Task<Success<BackgroundTaskInfo>> ImportBatch(ImportBatchModel model);
    }
}
