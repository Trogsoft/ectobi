using Microsoft.AspNetCore.Identity;

namespace Trogsoft.Ectobi.Data
{
    public class EctoUser : IdentityUser<Guid>
    {
        public ICollection<UserRefreshToken> RefreshTokens { get; set; } = new HashSet<UserRefreshToken>();
    }
}