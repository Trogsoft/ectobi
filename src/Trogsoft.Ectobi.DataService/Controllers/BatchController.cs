using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Trogsoft.Ectobi.Common;
using Trogsoft.Ectobi.Common.Interfaces;

namespace Trogsoft.Ectobi.DataService.Controllers
{
    [Route("api/batch")]
    [Authorize(Roles = "Administrator,BatchManager")]
    public class BatchController : EctoApiController
    {
        private readonly IBatchService batches;

        public BatchController(IBatchService batches) {
            this.batches = batches;
        }

        [HttpPost]
        public async Task<IActionResult> Import([FromBody] ImportBatchModel model)
            => SuccessResponse(await batches.ImportBatch(model));

        [HttpPost("empty")]
        public async Task<IActionResult> CreateEmptyBatch([FromBody] BatchModel model) 
            => SuccessResponse(await batches.CreateEmptyBatch(model));

        [HttpGet("{schemaTid}")]
        public async Task<IActionResult> GetBatchList(string schemaTid)
            => SuccessResponse(await batches.GetBatches(schemaTid));

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBatch(long id)
            => SuccessResponse(await batches.DeleteBatch(id));

    }
}
