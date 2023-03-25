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
        Success ExecutePopulators(long id);
        Task<Success<BackgroundTaskInfo>> ImportBatch(ImportBatchModel model);
    }
}
