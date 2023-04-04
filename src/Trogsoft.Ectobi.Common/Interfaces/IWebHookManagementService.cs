using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trogsoft.Ectobi.Common.Interfaces
{
    public interface IWebHookManagementService
    {
        Task<Success<WebHookModel>> GetWebHook(long id);
        Task<Success<List<WebHookModel>>> GetWebHooks();
    }
}
