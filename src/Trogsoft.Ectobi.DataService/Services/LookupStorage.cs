using Trogsoft.Ectobi.Data;
using Trogsoft.Ectobi.DataService.Interfaces;

namespace Trogsoft.Ectobi.DataService.Services
{
    public class LookupStorage : ILookupStorage
    {
        private readonly ILogger<ILookupStorage> logger;
        private readonly EctoDb db;

        public LookupStorage(ILogger<ILookupStorage> logger, EctoDb db)
        {
            this.logger = logger;
            this.db = db;
        }



    }
}
