using Microsoft.EntityFrameworkCore;
using Trogsoft.Ectobi.Common;
using Trogsoft.Ectobi.Common.Interfaces;
using Trogsoft.Ectobi.Data;
using Trogsoft.Ectobi.DataService.Data;
using Trogsoft.Ectobi.DataService.Interfaces;

namespace Trogsoft.Ectobi.DataService.Services
{
    public class LookupService : ILookupService
    {
        private readonly ILogger<LookupService> logger;
        private readonly EctoDb db;
        private readonly IEctoMapper mapper;
        private readonly ILookupStorage ils;
        private readonly IEctoData data;

        public LookupService(ILogger<LookupService> logger, EctoDb db, IEctoMapper mapper, ILookupStorage ils, IEctoData data)
        {
            this.logger = logger;
            this.db = db;
            this.mapper = mapper;
            this.ils = ils;
            this.data = data;
        }

        public async Task<Success<IEnumerable<LookupSetModel>>> GetLookupSets(string? schemaTid = null)
            => new Success<IEnumerable<LookupSetModel>>(await data.Lookup.GetLookupSets(schemaTid));

        public async Task<Success<LookupSetModel>> GetLookupSet(string lookupTid)
        {
            try
            {
                var result = await data.Lookup.GetLookupSet(lookupTid);
                return new Success<LookupSetModel>(result);
            }
            catch (LookupSetNotFoundException)
            {
                return Success<LookupSetModel>.Error("Lookup set not found: " + lookupTid, ErrorCodes.ERR_NOT_FOUND);
            }
            catch (Exception ex)
            {
                return Success<LookupSetModel>.Error(ex.Message, ErrorCodes.ERR_UNSPECIFIED_ERROR);
            }
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
