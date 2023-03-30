using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trogsoft.Ectobi.Common
{
    public class BatchModel
    {
        public long Id { get; set; }
        public string? Source { get; set; }
        public string? SchemaTid { get; set; }
        public DateTime Created { get; set; }
        public BatchFlags Flags { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? TextId { get; set; }
    }
}
