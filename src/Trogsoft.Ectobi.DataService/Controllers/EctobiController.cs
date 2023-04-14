using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Formats.Asn1;
using Trogsoft.Ectobi.Common.Interfaces;
using Trogsoft.Ectobi.DataService.Services;

namespace Trogsoft.Ectobi.DataService.Controllers
{
    public class EctobiController : EctoApiController
    {
        private readonly ModuleManager mm;
        private readonly IWebHookManagementService whm;
        private readonly IEctoServer server;

        public EctobiController(ModuleManager mm, IWebHookManagementService whm, IEctoServer server)
        {   
            this.mm = mm;
            this.whm = whm;
            this.server = server;
        }

        [HttpGet, Route("api/ecto/server")]
        public async Task<IActionResult> GetServerInfo() => SuccessResponse(await server.GetServerModel());

        [HttpGet, Route("api/ecto/populator"), Authorize]
        public async Task<IActionResult> GetPopulators() => SuccessResponse(mm.GetPopulatorDefinitions());

        [HttpGet, Route("api/ecto/importer"), Authorize]
        public async Task<IActionResult> GetFileImporters() => SuccessResponse(mm.GetFileHandlers());

        #region Web Hook Management

        [HttpGet, Route("api/ecto/webhook"), Authorize]
        public async Task<IActionResult> GetWebHooks() => SuccessResponse(await whm.GetWebHooks());

        [HttpGet, Route("api/ecto/webhook/{id}"), Authorize]
        public async Task<IActionResult> GetWebHook(long id) => SuccessResponse(await whm.GetWebHook(id));

        #endregion 


    }
}
