using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trogsoft.Ectobi.Common
{
    public class EctoModelDefinition
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? TextId { get; set; }
        public List<EctoModelProperty> Properties { get; set; } = new List<EctoModelProperty>();
    }
}
