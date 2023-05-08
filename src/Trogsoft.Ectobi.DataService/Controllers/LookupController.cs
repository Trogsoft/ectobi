using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Trogsoft.Ectobi.Common;
using Trogsoft.Ectobi.Common.Interfaces;

namespace Trogsoft.Ectobi.DataService.Controllers
{
    [Route("api/lookup")]
    [Authorize(Roles = "Administrator,LookupManager")]
    public class LookupController : EctoApiController
    {
        private readonly ILookupService ls;

        public LookupController(ILookupService ls)
        {
            this.ls = ls;
        }

        [HttpGet]
        public async Task<IActionResult> GetLookupSets(string? schemaTid = null) => SuccessResponse(await ls.GetLookupSets(schemaTid));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLookupSet(string id) => SuccessResponse(await ls.GetLookupSet(id));

        [HttpPost]
        public async Task<IActionResult> CreateLookupSet([FromBody] LookupSetModel model) => SuccessResponse(await ls.CreateLookupSet(model)); 

    }
}
