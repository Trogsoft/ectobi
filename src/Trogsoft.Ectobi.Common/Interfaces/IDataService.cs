using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trogsoft.Ectobi.Common.Interfaces
{
    public interface IDataService
    {
        Task<Success<ValueMapWithMetadata>> GetData(DataQueryModel query);
        Task<Success<FieldFilterCollection>> GetFilters(string schema);
    }
}
