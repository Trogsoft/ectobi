using Microsoft.AspNetCore.Identity;
using Trogsoft.Ectobi.Common;
using Trogsoft.Ectobi.Common.Interfaces;
using Trogsoft.Ectobi.Data;

namespace Trogsoft.Ectobi.DataService.Services
{
    public class AuthService : IAuthService
    {
        //private readonly SignInManager<EctoUser> sim;

        //public AuthService(SignInManager<EctoUser> sim) 
        //{
        //    this.sim = sim;
        //}

        public async Task<Success<EctoToken>> Login(LoginModel model)
        {
            throw new NotImplementedException();
        }

    }
}
