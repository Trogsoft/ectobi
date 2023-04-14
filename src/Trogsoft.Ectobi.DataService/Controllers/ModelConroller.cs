using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Trogsoft.Ectobi.Common;
using Trogsoft.Ectobi.Common.Interfaces;

namespace Trogsoft.Ectobi.DataService.Controllers
{
    [Route("api/model"), Authorize]
    public class ModelConroller : EctoApiController
    {
        private readonly IModelService ms;

        public ModelConroller(IModelService ms)
        {
            this.ms = ms;
        }

        [HttpGet]
        public async Task<IActionResult> GetModelDefinitions()
            => SuccessResponse(await ms.GetModelDefinitions());

        [HttpPost("config")]
        public async Task<IActionResult> ConfigureModel(ModelConfigurationModel model)
            => SuccessResponse(await ms.ConfigureModel(model));

    }
}
