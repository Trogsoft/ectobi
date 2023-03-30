using Microsoft.AspNetCore.Mvc;
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


    }
}
