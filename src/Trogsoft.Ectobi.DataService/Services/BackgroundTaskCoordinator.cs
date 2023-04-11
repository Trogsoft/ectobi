using Hangfire;
using Microsoft.AspNetCore.SignalR;
using System.Linq.Expressions;
using Trogsoft.Ectobi.Common;
using Trogsoft.Ectobi.Common.Interfaces;

namespace Trogsoft.Ectobi.DataService.Services
{
    public class BackgroundTaskCoordinator : IBackgroundTaskCoordinator
    {
        private readonly IHubContext<EctoEventHub> hub;

        public BackgroundTaskCoordinator(IHubContext<EctoEventHub> hub)
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
