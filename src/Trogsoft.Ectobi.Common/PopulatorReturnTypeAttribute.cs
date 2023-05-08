using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trogsoft.Ectobi.Common
{
    public class PopulatorReturnTypeAttribute : Attribute
    {
        public PopulatorReturnTypeAttribute(PopulatorReturnType type)
        {
            Type = type;
        }

        public PopulatorReturnType Type { get; }
    }
}
