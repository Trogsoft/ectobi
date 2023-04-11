using Microsoft.EntityFrameworkCore;
using Trogsoft.Ectobi.Common;
using Trogsoft.Ectobi.Common.Interfaces;
using Trogsoft.Ectobi.Data;

namespace Trogsoft.Ectobi.DataService.Services
{
    public class DataService : IDataService
    {
        private readonly EctoDb db;

        public DataService(EctoDb db) 
        {
            this.db = db;
        }

        public async Task<Success<ValueMap>> GetData(DataQueryModel query)
        {
            var dataQuery = db.Records.AsQueryable();
            dataQuery = dataQuery.Where(x => x.Batch.SchemaVersion.Schema.TextId == query.SchemaTid);
            return await QueryToValueMap(dataQuery);            
        }

        private async Task<Success<ValueMap>> QueryToValueMap(IQueryable<Record> dataQuery)
        {
            var vm = new ValueMap();
            dataQuery = dataQuery.Include(x => x.Values).ThenInclude(x => x.SchemaFieldVersion).ThenInclude(x => x.SchemaField);
            List<string> headings = new List<string>();

            var i = 1;
            foreach (var record in dataQuery)
            {
                if (i == 1)
                    headings.AddRange(record.Values.Select(x => x.SchemaFieldVersion.Name ?? x.SchemaFieldVersion.TextId ?? "Field"));

                var row = new ValueMapRow();
                row.AddRange(record.Values.Select(x => x.RawValue));
                vm.Rows.Add(row);

                i++;
            }

            vm.Headings = headings;

            return new Success<ValueMap>(vm);

        }
    }
}
