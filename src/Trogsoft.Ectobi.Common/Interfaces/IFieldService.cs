using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trogsoft.Ectobi.Common.Interfaces
{
    public interface IFieldService
    {
        void AutoDetectSchemaFieldParameters(SchemaFieldEditModel x);
        Task<Success<SchemaFieldModel>> CreateField(string schemaTid, SchemaFieldEditModel model);
        Task<Success> DeleteField(string schemaTid, string fieldName);
        Task<Success<SchemaFieldEditModel>> GetField(string schemaTid, string fieldTid);
        Task<Success<List<SchemaFieldModel>>> GetFields(string schemaTid);
        Task<Success<List<SchemaFieldModel>>> GetVersionFields(string schemaTid, int version);
        void PopulateField(long fieldId);
    }
}
