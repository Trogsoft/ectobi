using Microsoft.AspNetCore.Mvc;
using Trogsoft.Ectobi.Common;

namespace Trogsoft.Ectobi.DataService.Controllers
{
    public class EctoApiController : ControllerBase
    {

        protected IActionResult SuccessResponse(Success result)
        {
            if (result.Succeeded)
                return new JsonResult(result);
            else
                return HttpStatusResult(result);
        }

        private IActionResult HttpStatusResult(Success result)
        {
            if (result.ErrorCode == ErrorCodes.ERR_NOT_FOUND) return NotFound(result);
            if (result.ErrorCode == ErrorCodes.ERR_ARGUMENT_NULL) return BadRequest(result);
            return StatusCode(500, result);
        }

        protected IActionResult SuccessResponse<T>(Success<T> result)
        {
            if (result.Succeeded)
                return new JsonResult(result);
            else
                return HttpStatusResult(result);
        }

    }
}