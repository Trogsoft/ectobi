using Hangfire;
using System.Linq.Expressions;
using Trogsoft.Ectobi.Common;
using Trogsoft.Ectobi.Common.Interfaces;

namespace Trogsoft.Ectobi.DataService.Services
{
    public class BackgroundTaskCoordinator : IBackgroundTaskCoordinator
    {
        private readonly EctoEventHub hub;

        public BackgroundTaskCoordinator(EctoEventHub hub)
        {
            this.hub = hub;
        }

        public void Enqueue<T>(Expression<Action<T>> expr)
        {
            BackgroundJob.Enqueue<T>(expr);
        }

        public void NotifyTaskBegun(BackgroundTaskInfo bgti)
        {
        }        

    }
}
