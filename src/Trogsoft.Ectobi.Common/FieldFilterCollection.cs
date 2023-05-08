using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trogsoft.Ectobi.Common
{
    public class FieldFilterCollection 
    {
        public DataQueryModel Query { get; set; } = new DataQueryModel();
        public List<FieldFilterModel> FieldFilters { get; set; } = new List<FieldFilterModel>();
    }
}
