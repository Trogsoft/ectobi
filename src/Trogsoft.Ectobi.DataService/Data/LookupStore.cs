using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Trogsoft.Ectobi.Common;
using Trogsoft.Ectobi.Data;

namespace Trogsoft.Ectobi.DataService.Data
{
    internal class LookupStore : ILookupStore
    {
        private readonly IEctoData data;

        public LookupStore(IEctoData data)
        {
            this.data = data;
        }

        public async Task<long> GetLookupSetId(string tid)
            => (await data.Store.LookupSets.SingleOrDefaultAsync(x => x.TextId == tid))?.Id ?? throw new LookupSetNotFoundException();

        public async Task<IEnumerable<LookupSetModel>> FindMatchingLookupSets(LookupSetModel model)
        {

            var valueNames = model.Values.Select(x => x.Name).ToList();
            var values = model.Values.Select(x => x.Value).ToList();

            var dataSet = await data.Store.LookupSets
                .Include(x => x.Values)
                .Where(x => (x.Schema == null || x.Schema.TextId == model.SchemaTid))
                .Where(x => x.Values.Count == model.Values.Count)
                .Where(x => x.Values.Any(y => valueNames.Contains(y.Name)))
                .Where(x => x.Values.Any(y => values.Contains(y.NumericValue)))
                .ToListAsync();

            if (dataSet.Count == 0)
                return new List<LookupSetModel>();

            return dataSet.Select(x => data.Mapper.Map<LookupSet, LookupSetModel>(x)).ToList();

        }

        public async Task<LookupSetModel> CreateLookupSet(LookupSetModel model)
        {
            model.TextId = data.Store.GetTextId<LookupSet>(model.Name ?? "Untitled Lookup");
            var entity = data.Mapper.Map<LookupSetModel, LookupSet>(model);

            if (model.SchemaTid != null)
            {
                var schemaId = await data.Schema.GetSchemaId(model.SchemaTid);
                entity.SchemaId = schemaId;
            }

            data.Store.LookupSets.Add(entity);
            await data.Store.SaveChangesAsync();
            return data.Mapper.Map<LookupSet, LookupSetModel>(entity);
        }

        public async Task<IEnumerable<LookupSetModel>> GetLookupSets(string? schemaTid = null)
        {
            var query = data.Store.LookupSets.Include(x => x.Values).AsQueryable();
            if (schemaTid != null)
                query = query.Where(x => x.Schema.TextId == schemaTid);
            return await query.Select(x => data.Mapper.Map<LookupSet, LookupSetModel>(x)).ToListAsync();
        }

        public async Task<bool> LookupSetExists(string tid)
            => await data.Store.LookupSets.AnyAsync(x => x.TextId == tid);

        public async Task<LookupSetModel> GetLookupSet(string tid)
        {
            var set = await data.Store.LookupSets.Include(x => x.Values).SingleOrDefaultAsync(x => x.TextId == tid);
            if (set == null)
                throw new LookupSetNotFoundException();
            return data.Mapper.Map<LookupSet, LookupSetModel>(set);
        }

        public async Task<bool> DeleteLookupSet(string tid)
        {
            if (!(await LookupSetExists(tid))) throw new LookupSetNotFoundException();
            var entity = await data.Store.LookupSets.SingleOrDefaultAsync(x => x.TextId == tid);
            if (entity == null) return true;

            data.Store.LookupSetValues.RemoveRange(await data.Store.LookupSetValues.Where(x => x.LookupSetId == entity.Id).ToListAsync());
            data.Store.LookupSets.Remove(entity!);
            await data.Store.SaveChangesAsync();
            return true;
        }

    }
}