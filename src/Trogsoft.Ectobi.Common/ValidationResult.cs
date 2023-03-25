using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trogsoft.Ectobi.Common
{
    public class ValidationResult
    {
        public bool Failed { get; set; } = false;
        public string? Message { get; set; }
    }
}
