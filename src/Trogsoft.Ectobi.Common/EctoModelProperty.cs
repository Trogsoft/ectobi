using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trogsoft.Ectobi.Common
{
    public class EctoModelProperty
    {
        public string? Name { get; set; }
        public string? TextId { get; set; }
        public string? Description { get; set; }
        public SchemaFieldType Type { get; set; }
        public EctoModelPropertyFlags Flags { get; set; }
    }
}
