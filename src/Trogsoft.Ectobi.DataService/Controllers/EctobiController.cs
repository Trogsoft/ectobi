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

        public EctobiController(ModuleManager mm, IWebHookManagementService whm)
        {
            this.mm = mm;
            this.whm = whm;
        }

        [HttpGet, Route("api/ecto/populator")]
        public async Task<IActionResult> GetPopulators() => SuccessResponse(mm.GetPopulatorDefinitions());

        [HttpGet, Route("api/ecto/importer")]
        public async Task<IActionResult> GetFileImporters() => SuccessResponse(mm.GetFileHandlers());

        #region Web Hook Management

        [HttpGet, Route("api/ecto/webhook")]
        public async Task<IActionResult> GetWebHooks() => SuccessResponse(await whm.GetWebHooks());

        [HttpGet, Route("api/ecto/webhook/{id}")]
        public async Task<IActionResult> GetWebHook(long id) => SuccessResponse(await whm.GetWebHook(id));

        #endregion 


    }
}
