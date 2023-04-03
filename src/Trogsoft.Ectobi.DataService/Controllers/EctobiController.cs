using Microsoft.AspNetCore.Mvc;
using System.Formats.Asn1;
using Trogsoft.Ectobi.DataService.Services;

namespace Trogsoft.Ectobi.DataService.Controllers
{
    public class EctobiController : EctoApiController
    {
        private readonly ModuleManager mm;

        public EctobiController(ModuleManager mm)
        {
            this.mm = mm;
        }

        [Route("api/ecto/populator")]
        public async Task<IActionResult> GetPopulators() => SuccessResponse(mm.GetPopulatorDefinitions());

        [Route("api/ecto/importer")]
        public async Task<IActionResult> GetFileImporters() => SuccessResponse(mm.GetFileHandlers());

    }
}
