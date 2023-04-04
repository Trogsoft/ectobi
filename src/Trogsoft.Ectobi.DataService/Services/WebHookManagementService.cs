using Microsoft.EntityFrameworkCore;
using Trogsoft.Ectobi.Common;
using Trogsoft.Ectobi.Common.Interfaces;
using Trogsoft.Ectobi.Data;

namespace Trogsoft.Ectobi.DataService.Services
{
    public class WebHookManagementService : IWebHookManagementService
    {
        private readonly EctoDb db;
        private readonly IEctoMapper mapper;

        public WebHookManagementService(EctoDb db, IEctoMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<Success<List<WebHookModel>>> GetWebHooks()
        {
            List<WebHookModel> list = new List<WebHookModel>();
            foreach (var wh in await db.WebHooks.ToListAsync())
            {
                list.Add(mapper.Map<WebHookModel>(wh));
            }
            return new Success<List<WebHookModel>>(list);
        }

        public async Task<Success<WebHookModel>> GetWebHook(long id)
        {
            var webHook = await db.WebHooks.SingleOrDefaultAsync(x => x.Id == id);
            if (webHook == null) return Success<WebHookModel>.Error("Webhook not found.", ErrorCodes.ERR_NOT_FOUND);

            return new Success<WebHookModel>(mapper.Map<WebHookModel>(webHook));
        }

    }
}
