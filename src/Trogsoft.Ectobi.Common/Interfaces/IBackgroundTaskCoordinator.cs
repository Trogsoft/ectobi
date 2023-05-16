using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trogsoft.Ectobi.Common.Interfaces
{
    public interface IBackgroundTaskCoordinator
    {
        void Enqueue<T>(System.Linq.Expressions.Expression<Action<T>> expr);
        void Schedule<T>(System.Linq.Expressions.Expression<Action<T>> expr, TimeSpan delay);

        void Log(BackgroundTaskInfo bgti, string message);
        void TaskBegun(BackgroundTaskInfo bgti);
        void TaskCompleted(BackgroundTaskInfo bgti);
        void TaskFailed(BackgroundTaskInfo bgti);
        void TaskProgressChanged(BackgroundTaskInfo bgti, int total, int current);
        void TaskStatusChanged(BackgroundTaskInfo bgti, string status);
        Task TaskBegunAsync(BackgroundTaskInfo bgti);
        Task TaskProgressChangedAsync(BackgroundTaskInfo bgti, int total, int current);
        Task TaskStatusChangedAsync(BackgroundTaskInfo bgti, string status);
        Task LogAsync(BackgroundTaskInfo bgti, string message);
        Task TaskCompletedAsync(BackgroundTaskInfo bgti);
        Task TaskFailedAsync(BackgroundTaskInfo bgti);
    }
}
