using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trogsoft.Ectobi.Common.Interfaces
{
    public interface ISchemaService
    {
        Task<Success<SchemaModel>> CreateSchema(SchemaEditModel model);
        Task<Success<SchemaVersionModel>> CreateSchemaVersion(SchemaVersionEditModel model);
        Task<Success> DeleteSchema(string schemaTextId);
        Task<Success<SchemaModel>> GetSchema(string textId);
        Task<Success<List<SchemaModel>>> GetSchemas(bool includeDetails = false);
        Task<Success<List<SchemaVersionModel>>> GetSchemaVersions(string schemaTid);
    }
}
