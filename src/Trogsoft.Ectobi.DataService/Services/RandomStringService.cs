using System.Security.Cryptography;
using Trogsoft.Ectobi.DataService.Interfaces;

namespace Trogsoft.Ectobi.DataService.Services
{
    public class RandomStringService : IRandomStringService
    {
        public RandomStringService()
        {

        }

        public string GetRandomString(int length = 32)
        {
            string secureRandomString = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            return secureRandomString;
        }

    }
}
