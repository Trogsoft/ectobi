using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trogsoft.Ectobi.Common
{
    public class ImportBatchModel
    {
        public string? SchemaTid { get; set; }
        public string? BatchName { get; set; }
        public string? BatchSource { get; set; }

        public List<ValueMap> Rows { get; set; } = new();
    }
}
