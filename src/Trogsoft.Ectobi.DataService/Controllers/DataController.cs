using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Trogsoft.Ectobi.Common;
using Trogsoft.Ectobi.Common.Interfaces;

namespace Trogsoft.Ectobi.DataService.Controllers
{
    [Authorize]
    [Route("api/data")]
    public class DataController : EctoApiController
    {

        private readonly IDataService data;

        public DataController(IDataService data)
        {
            this.data = data;
        }

        [HttpPost("query")]
        public async Task<IActionResult> GetData([FromBody] DataQueryModel query) =>
            SuccessResponse(await data.GetData(query));


        [HttpGet("fieldFilter/{schema}")]
        public async Task<IActionResult> GetFieldFilters(string schema) =>
            SuccessResponse(await data.GetFilters(schema));

    }
}
