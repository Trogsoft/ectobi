using Hangfire;
using Hangfire.Annotations;
using Microsoft.AspNetCore.SignalR;
using System.Linq.Expressions;
using Trogsoft.Ectobi.Common;
using Trogsoft.Ectobi.Common.Interfaces;

namespace Trogsoft.Ectobi.DataService.Services
{
    public class BackgroundTaskCoordinator : IBackgroundTaskCoordinator
    {
        private readonly IHubContext<EctoEventHub> hub;
        private readonly IServiceScopeFactory issf;
        private readonly IEctoServer server;
        private bool authEnabled = true;

        public BackgroundTaskCoordinator(IHubContext<EctoEventHub> hub, IServiceScopeFactory issf)
        {
            this.hub = hub;
            this.issf = issf;
            using (var scope = issf.CreateScope())
            {
                this.server = scope.ServiceProvider.GetRequiredService<IEctoServer>();
                var serverModel = server.GetServerModel().Result;
                if (serverModel.Succeeded && serverModel.Result != null)
                {
                    this.authEnabled = serverModel.Result.RequiresLogin;
                }
            }
        }

        public void Enqueue<T>(Expression<Action<T>> expr) 
            => BackgroundJob.Enqueue<T>(expr);

        public void Schedule<T>(Expression<Action<T>> expr, TimeSpan delay)
            => BackgroundJob.Schedule(expr, delay);


        public async Task TaskBegunAsync(BackgroundTaskInfo bgti)
        {
            await GetTargetAudience(bgti).SendAsync("taskBegun", bgti);
        }

        public void TaskBegun(BackgroundTaskInfo bgti) => TaskBegunAsync(bgti).Wait();

        public async Task TaskProgressChangedAsync(BackgroundTaskInfo bgti, int total, int current)
        {
            await GetTargetAudience(bgti).SendAsync("taskProgressChanged", bgti, total, current);
        }

        public void TaskProgressChanged(BackgroundTaskInfo bgti, int total, int current) => TaskProgressChangedAsync(bgti, total, current).Wait();

        public async Task TaskStatusChangedAsync(BackgroundTaskInfo bgti, string status)
        {
            await GetTargetAudience(bgti).SendAsync("taskStatusChanged", bgti, status);
        }

        public void TaskStatusChanged(BackgroundTaskInfo bgti, string status) => TaskStatusChangedAsync(bgti, status).Wait();

        public async Task LogAsync(BackgroundTaskInfo bgti, string message)
        {
            await GetTargetAudience(bgti).SendAsync("taskLog", bgti, message);
        }

        public void Log(BackgroundTaskInfo bgti, string message) => LogAsync(bgti, message).Wait();

        public async Task TaskCompletedAsync(BackgroundTaskInfo bgti)
        {
            await GetTargetAudience(bgti).SendAsync("taskCompleted", bgti);
        }

        public void TaskCompleted(BackgroundTaskInfo bgti) => TaskCompletedAsync(bgti).Wait();

        public async Task TaskFailedAsync(BackgroundTaskInfo bgti)
        {
            await GetTargetAudience(bgti).SendAsync("taskFailed", bgti);
        }

        public void TaskFailed(BackgroundTaskInfo bgti) => TaskFailedAsync(bgti).Wait();

        private IClientProxy GetTargetAudience(BackgroundTaskInfo bgti)
        {
            if (!this.authEnabled)
                return hub.Clients.All;

            return hub.Clients.User(bgti.UserId?.ToString() ?? Guid.Empty.ToString());
        }
    }
}
