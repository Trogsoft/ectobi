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
    }
}
