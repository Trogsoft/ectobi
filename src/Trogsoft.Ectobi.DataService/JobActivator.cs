using Hangfire;
using System.ComponentModel;

namespace Trogsoft.Ectobi.DataService
{
    public class EctoJobActivator : JobActivator
    {
        private readonly IServiceProvider _serviceProvider;

        public EctoJobActivator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public override object ActivateJob(Type type)
        {
            return _serviceProvider.GetService(type)!;
        }
    }
}
