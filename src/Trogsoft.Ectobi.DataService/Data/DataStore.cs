using Microsoft.EntityFrameworkCore;

namespace Trogsoft.Ectobi.DataService.Data
{
    public class DataStore : IDataStore
    {
        private EctoData data;

        public DataStore(EctoData ectoData)
        {
            this.data = ectoData;
        }

        public async Task DeleteAllFieldValues(string schemaTid, string fieldTid, int version = 0)
        {

            var schemaVersion = await data.Schema.GetSchemaVersion(schemaTid, version);

            var values = data.Store.Values.Where(x => x.SchemaFieldVersion.SchemaField.TextId == fieldTid
                && x.SchemaFieldVersion.SchemaField.Schema.TextId == schemaTid
                && x.SchemaFieldVersion.SchemaVersion.Version == schemaVersion.Version);
            var anyValues = await values.AnyAsync();

            if (!anyValues) return;

            data.Store.Values.RemoveRange(values);
            await data.Store.SaveChangesAsync();

        }

    }
}