using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trogsoft.Ectobi.Common
{
    public class SchemaModel
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? TextId { get; set; }
        public List<SchemaFieldModel> Fields { get; set; } = new List<SchemaFieldModel>();
    }

    public class SchemaEditModel : SchemaModel
    {
        public bool Overwrite { get; set; } = false;
        public bool AutoDetect { get; set; } = true;
        public new List<SchemaFieldEditModel> Fields { get; set; } = new List<SchemaFieldEditModel>();
        public BinaryFileModel File { get; set; } = new BinaryFileModel();
    }


}
