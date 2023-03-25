using Trogsoft.Ectobi.Common;
using Trogsoft.Ectobi.Common.Interfaces;
using Trogsoft.Ectobi.Data;
using Trogsoft.Ectobi.DataService.Interfaces;

namespace Trogsoft.Ectobi.DataService.Services
{
    public class LookupService : ILookupService
    {
        private readonly ILogger<LookupService> logger;
        private readonly EctoDb db;
        private readonly IEctoMapper mapper;
        private readonly ILookupStorage ils;

        public LookupService(ILogger<LookupService> logger, EctoDb db, IEctoMapper mapper, ILookupStorage ils)
        {
            this.logger = logger;
            this.db = db;
            this.mapper = mapper;
            this.ils = ils;
        }

        public async Task<Success<LookupSetModel>> CreateLookupSet(LookupSetModel model)
        {

            if (model == null) return Success<LookupSetModel>.Error("Model cannot be null.", ErrorCodes.ERR_ARGUMENT_NULL);
            if (string.IsNullOrWhiteSpace(model.Name)) return Success<LookupSetModel>.Error("Model.Name cannot be null.", ErrorCodes.ERR_ARGUMENT_NULL);

            var lookupSet = new LookupSet
            {
                Name = model.Name,
                Description = model.Description,
                TextId = db.GetTextId<LookupSet>(model.Name),
            };

            if (model.Values != null && model.Values.Any())
            {
                model.Values.ForEach(x =>
                {
                    lookupSet.Values.Add(new LookupSetValue
                    {
                        Name = x.Name,
                        NumericValue = x.Value
                    });
                });
            }

            db.LookupSets.Add(lookupSet);
            await db.SaveChangesAsync();

            return new Success<LookupSetModel>(mapper.Map<LookupSetModel>(lookupSet));

        }

    }
}
