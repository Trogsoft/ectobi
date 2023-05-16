using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trogsoft.Ectobi.Common
{
    public class PopulatorModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string TextId { get; set; }
        public IList<PopulatorOption> Options { get; set; } = new List<PopulatorOption>();
    }
}
