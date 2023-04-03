using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trogsoft.Ectobi.Common
{
    public enum WebHookEventStatus
    {
        Success = 1,
        Failed = 2,
        Pending = 3,
        RetryPending = 4
    }
}
