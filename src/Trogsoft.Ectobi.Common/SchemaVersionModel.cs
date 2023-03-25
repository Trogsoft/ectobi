using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trogsoft.Ectobi.Common
{
    public class SchemaVersionModel
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public int Version { get; set; }
        public DateTime Created { get; set; }
        public string? Description { get; set; }
        public List<SchemaFieldModel> Fields { get; set; } = new List<SchemaFieldModel>();
        public string SchemaTid { get; set; }
    }
}
