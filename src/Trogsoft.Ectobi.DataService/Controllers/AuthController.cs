using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Trogsoft.Ectobi.Common;
using Trogsoft.Ectobi.Common.Interfaces;
using Trogsoft.Ectobi.Data;

namespace Trogsoft.Ectobi.DataService.Controllers
{
    [Route("auth")]
    public class AuthController : EctoApiController
    {
        private readonly IAuthService ias;

        public AuthController(IAuthService ias) 
        {
            this.ias = ias;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel model) => SuccessResponse(await ias.Login(model));

    }
}
