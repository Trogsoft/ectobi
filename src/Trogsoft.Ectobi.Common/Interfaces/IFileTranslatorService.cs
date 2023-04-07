using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trogsoft.Ectobi.Common.Interfaces
{
    public interface IFileTranslatorService
    {
        Task<Success<List<SchemaFieldEditModel>>> GetSchemaFieldEditModelCollection(BinaryFileModel file);
        Task<Success<ValueMap>> GetValueMap(BinaryFileModel binaryFile);
    }
}
