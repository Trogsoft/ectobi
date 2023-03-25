using System.Security.Cryptography;
using Trogsoft.Ectobi.Common;
using Trogsoft.Ectobi.Common.Interfaces;

namespace Trogsoft.Ectobi.DataService.Populators
{
    public class RandomStringPopulator : IPopulator
    {
        public string GetString()
        {
            //var count = values.GetValue<int>("length", 32);
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        }
    }
}
