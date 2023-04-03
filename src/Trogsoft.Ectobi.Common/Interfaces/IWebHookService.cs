using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trogsoft.Ectobi.Common.Interfaces
{
    public interface IWebHookService
    {
        Task Dispatch<TEvent>(WebHookEventType type, TEvent webHookEvent);
        void ExecuteTask(long id);
    }
}
