using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Trogsoft.Ectobi.Common;
using Trogsoft.Ectobi.Common.Interfaces;
using Trogsoft.Ectobi.Data;
using Trogsoft.Ectobi.DataService.Data;
using Trogsoft.Ectobi.DataService.Validation;

namespace Trogsoft.Ectobi.DataService.Services
{
    public class DataService : IDataService
    {
        private readonly EctoDb db;
        private readonly IEctoData data;

        public DataService(EctoDb db, IEctoData data)
        {
            this.db = db;
            this.data = data;
        }

        public async Task<Success<ValueMapWithMetadata>> GetData(DataQueryModel query)
        {

            var validator = EctoModelValidator.CreateValidator<DataQueryModel>(db)
                .WithModel(query)
                .Property(x => x.SchemaTid).NotNullOrWhiteSpace()
                .Entity<Schema>(x => x.TextId == query.SchemaTid).MustExist();

            if (!validator.Validate()) return validator.GetResult<ValueMapWithMetadata>();

            var schemaVersion = db.SchemaVersions.OrderByDescending(x => x.Created).FirstOrDefault(x => x.Schema.TextId == query.SchemaTid);
            if (schemaVersion == null)
                return Success<ValueMapWithMetadata>.Error("No schema versions defined.");

            var fields = await db.SchemaFieldVersions.Include(x=>x.SchemaField).Where(x => x.SchemaVersion != null && 
                x.SchemaVersion.Schema.TextId == query.SchemaTid && 
                x.SchemaVersion.Version == schemaVersion.Version)
            .ToListAsync();

            var dataQuery = db.Records.AsQueryable();

            dataQuery = dataQuery.Where(x => x.Batch.SchemaVersion.Schema.TextId == query.SchemaTid);

            foreach (var key in query.Filter.Keys)
            {
                var field = fields.SingleOrDefault(x => x.SchemaField.TextId == key);
                if (field == null) return Success<ValueMapWithMetadata>.Error("Query parameter not recognised: " + key);
                if (field.Type == SchemaFieldType.Set)
                {
                    var lookupIds = query.GetNumeric(key);
                    dataQuery = dataQuery.Where(x => x.Values.Any(y => y.SchemaFieldVersion.SchemaField.TextId == key && y.IntegerValue != null && lookupIds.Contains(y.IntegerValue.Value)));
                }
                else
                {
                    var value = query.Filter[key];
                    dataQuery = dataQuery.Where(x => x.Values.Any(y => y.SchemaFieldVersion.SchemaField.TextId == key && value.Contains(y.RawValue)));
                }
            }

            var totalRows = await dataQuery.CountAsync();

            if (query.RecordsPerPage.HasValue)
            {
                dataQuery = dataQuery.OrderBy(x => x.Created);

                if (query.Page > 1)
                    dataQuery = dataQuery.Skip(query.RecordsPerPage.Value * (query.Page - 1));

                dataQuery = dataQuery.Take(query.RecordsPerPage.Value);
            }

            return await QueryToValueMap(dataQuery, totalRows);
        }

        public async Task<Success<FieldFilterCollection>> GetFilters(string schema)
        {

            var schemaExists = await data.Schema.SchemaExists(schema);
            if (!schemaExists) return Success<FieldFilterCollection>.Error("Schema does not exist.", ErrorCodes.ERR_NOT_FOUND);

            var schemaVersion = await data.Schema.GetLatestVersionEntityId(schema);

            var fc = new FieldFilterCollection();
            var setFields = await db.SchemaFieldVersions
                .Include(x => x.SchemaField)
                .Include(x => x.LookupSet).ThenInclude(x => x.Values)
                .Where(x => x.SchemaVersion != null && x.SchemaVersion.Version == schemaVersion)
                .Where(x => x.Type == SchemaFieldType.Set)
                .ToListAsync();

            foreach (var field in setFields)
            {
                var filterModel = data.Mapper.Map<SchemaFieldVersion, FieldFilterModel>(field);
                filterModel.Type = FieldFilterType.Set;
                fc.FieldFilters.Add(filterModel);
                fc.Query.Set(filterModel.TextId, filterModel.Options.Select(x => x.Id));
            }

            fc.Query.SchemaTid = schema;

            return new Success<FieldFilterCollection>(fc);

        }

        private async Task<Success<ValueMapWithMetadata>> QueryToValueMap(IQueryable<Record> dataQuery, int totalRowCount)
        {
            var vm = new ValueMapWithMetadata();
            dataQuery = dataQuery.Include(x => x.Values).ThenInclude(x => x.SchemaFieldVersion).ThenInclude(x => x.SchemaField);
            List<string> headings = new List<string>();

            vm.TotalRowsForQuery = totalRowCount;
            var sw = new Stopwatch();
            sw.Start();

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

            sw.Stop();
            
            vm.Headings = headings;
            vm.TimeTaken = TimeSpan.FromMilliseconds(sw.ElapsedMilliseconds);

            return new Success<ValueMapWithMetadata>(vm);

        }
    }
}
