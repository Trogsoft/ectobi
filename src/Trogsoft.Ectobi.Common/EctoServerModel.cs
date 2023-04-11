using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trogsoft.Ectobi.Common
{
    public class EctoServerModel
    {
        public bool RequiresLogin { get; set; }
        public string? Name { get; set; }
        public EctoUserCapabilitiesModel UserCapabilities { get; set; } = new EctoUserCapabilitiesModel();
    }
}
