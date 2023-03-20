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
        Task<Success<List<SchemaFieldModel>>> GetFields(string schemaTid);
    }
}
