using Hangfire;
using System.Linq.Expressions;
using Trogsoft.Ectobi.Common.Interfaces;

namespace Trogsoft.Ectobi.DataService.Services
{
    public class BackgroundJobCoordinator : IBackgroundJobCoordinator
    {
        public void Enqueue<T>(Expression<Action<T>> expr)
        {
            BackgroundJob.Enqueue<T>(expr);
        }
    }
}
