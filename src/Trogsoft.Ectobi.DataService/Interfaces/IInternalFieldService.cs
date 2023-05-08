using Trogsoft.Ectobi.Common;
using Trogsoft.Ectobi.Common.Interfaces;

namespace Trogsoft.Ectobi.DataService.Interfaces
{
    public interface IInternalFieldService : IFieldService
    {
        void PopulateField(long fieldId);
        void AutoDetectSchemaFieldParameters(SchemaFieldEditModel x);
    }
}
