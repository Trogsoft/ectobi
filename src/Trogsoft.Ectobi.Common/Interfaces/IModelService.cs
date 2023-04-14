using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trogsoft.Ectobi.Common.Interfaces
{
    public interface IModelService
    {
        Task<Success<List<SchemaFieldModel>>> ConfigureModel(ModelConfigurationModel model);
        Task<Success<List<EctoModelDefinition>>> GetModelDefinitions();
    }
}
