using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trogsoft.Ectobi.Common
{
    public class ModelConfigurationModel
    {
        public string? SchemaTid { get; set; }
        public string? ModelName { get; set; }
        public List<string> Properties { get; set; } = new List<string>();
    }
}
