using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trogsoft.Ectobi.Common
{
    public class ModelFlagsAttribute : Attribute
    {
        public ModelFlagsAttribute(EctoModelPropertyFlags flags)
        {
            Flags = flags;
        }

        public EctoModelPropertyFlags Flags { get; }
    }
}
