using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trogsoft.Ectobi.Common.Interfaces
{
    public interface IValueObject
    {
        long? IntegerValue { get; set; }
        string RawValue { get; set; }
        bool? BoolValue { get; set; }
        double? DecimalValue { get; set; }
    }
}
