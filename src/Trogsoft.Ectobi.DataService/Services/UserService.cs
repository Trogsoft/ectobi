using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Trogsoft.Ectobi.Common;
using Trogsoft.Ectobi.Common.Interfaces;
using Trogsoft.Ectobi.Data;
using Trogsoft.Ectobi.DataService.Interfaces;

namespace Trogsoft.Ectobi.DataService.Services
{
    public class UserService : IAuthService
    {
        private readonly UserManager<EctoUser> um;
        private readonly SignInManager<EctoUser> sim;
        private readonly IConfiguration config;
        private readonly IRandomStringService irs;
        private readonly EctoDb db;

        public UserService(UserManager<EctoUser> um, SignInManager<EctoUser> sim, IConfiguration config, IRandomStringService irs, EctoDb db)
        {
            this.um = um;
            this.sim = sim;
            this.config = config;
            this.irs = irs;
            this.db = db;
        }

        public async Task<Success<EctoToken>> RefreshToken(EctoToken token)
        {

            var principal = GetPrincipalFromExpiredToken(token.Token);
            var username = principal.Identity.Name;

            var user = await um.FindByEmailAsync(username);
            if (user == null)
                return Success<EctoToken>.Error("User not found.");

            var dbUser = db.Users.Include(x => x.RefreshTokens).SingleOrDefault(x => x.Id == user.Id);
            if (dbUser == null)
                return Success<EctoToken>.Error("User not found.");

            var dbRefreshToken = dbUser.RefreshTokens.SingleOrDefault(x => x.Token == token.RefreshToken);
            if (dbRefreshToken == null)
                return Success<EctoToken>.Error("Refresh token is invalid.");

            var newToken = await GenerateJwtToken(user);
            var newRefreshToken = irs.GetRandomString(48);

            dbRefreshToken.Token = newRefreshToken;
            dbRefreshToken.Expires = DateTime.Now.AddDays(7);
            db.SaveChanges();

            return new Success<EctoToken>(new EctoToken
            {
                RefreshToken = newRefreshToken,
                Token = newToken
            });

        }

        private SymmetricSecurityKey GetJwtSecret()
        {
            var secret = config.GetSection("Ectobi").GetValue<string>("JwtSecret");
            var key = Encoding.UTF8.GetBytes(secret);
            return new SymmetricSecurityKey(key);
        }

        // From: https://code-maze.com/using-refresh-tokens-in-asp-net-core-authentication/
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = GetJwtSecret(),
                ValidateLifetime = false
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");
            return principal;
        }

        private async Task<string> GenerateJwtToken(EctoUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email)
            };

            var signingCredentials = new SigningCredentials(GetJwtSecret(), SecurityAlgorithms.HmacSha256);

            var roles = await um.GetRolesAsync(user);
            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var tokenOptions = new JwtSecurityToken(
                issuer: "ectobi",
                audience: "ectobi",
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(5)),
                signingCredentials: signingCredentials);

            var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            return token;
        }

        public async Task<Success<EctoToken>> Login(LoginModel model)
        {
            var user = await um.FindByNameAsync(model.Username);
            if (user == null) return Success<EctoToken>.Error("Unauthorised", ErrorCodes.ERR_LOGIN_FAILED);

            var result = await um.CheckPasswordAsync(user, model.Password);
            if (result)
            {

                var token = await GenerateJwtToken(user);
                if (token == null) return Success<EctoToken>.Error("Failed", ErrorCodes.ERR_LOGIN_FAILED);

                var refreshToken = irs.GetRandomString(64);

                db.RefreshTokens.Add(new UserRefreshToken
                {
                    Created = DateTime.Now,
                    Expires = DateTime.Now.AddDays(30),
                    SessionIdentifier = model.SessionIdentifier ?? "Default",
                    Token = refreshToken,
                    UserId = user.Id
                });
                await db.SaveChangesAsync();

                return new Success<EctoToken>(new EctoToken { Token = token, RefreshToken = refreshToken });
            }

            return Success<EctoToken>.Error("Failed.", ErrorCodes.ERR_LOGIN_FAILED);

        }

        //public async Task<Success<EctoUserModel>>

    }
}
