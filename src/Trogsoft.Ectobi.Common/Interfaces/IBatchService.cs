using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trogsoft.Ectobi.Common.Interfaces
{
    public interface IBatchService
    {
        Success BackgroundImportBatch(BackgroundTaskInfo job, string temporaryFile);
        Task<Success<BatchModel>> CreateEmptyBatch(BatchModel model);
        Task<Success> DeleteBatch(long batchId);
        Success ExecutePopulators(BackgroundTaskInfo bgti, long id);
        Task<Success<List<BatchModel>>> GetBatches(string schemaTid);
        Task<Success<BackgroundTaskInfo>> ImportBatch(ImportBatchModel model);
    }
}
