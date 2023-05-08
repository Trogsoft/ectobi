using Trogsoft.Ectobi.Common;
using Trogsoft.Ectobi.Data;

namespace Trogsoft.Ectobi.DataService.Data
{
    public interface IFieldStore
    {
        Task<SchemaField> CreateRootField(SchemaFieldEditModel model);
        Task<SchemaFieldModel> CreateVersionField(SchemaFieldEditModel model);
        Task<bool> DeleteField(string schemaTid, string fieldTid, int version = 0);
        Task<bool> DeleteRootField(string schemaTid, string fieldTid);
        Task<SchemaFieldModel> GetRootField(string schemaTid, string fieldTid);
        Task<bool> OtherFieldVersionsExist(string schemaTid, string fieldTid, int version);
        Task SetFieldModel(SchemaFieldModel versionField, string modelName, string modelField);
        Task SetFieldPopulator(SchemaFieldModel versionField, string populator);
    }
}