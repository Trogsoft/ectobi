using Microsoft.AspNetCore.Mvc;
using Trogsoft.Ectobi.Common;
using Trogsoft.Ectobi.Common.Interfaces;

namespace Trogsoft.Ectobi.DataService.Controllers
{
    [Route("api/batch")]
    public class BatchController : EctoApiController
    {
        private readonly IBatchService batches;

        public BatchController(IBatchService batches) {
            this.batches = batches;
        }

        [HttpPost]
        public async Task<IActionResult> Import([FromBody] ImportBatchModel model)
            => SuccessResponse(await batches.ImportBatch(model));

    }
}
