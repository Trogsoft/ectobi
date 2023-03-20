using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trogsoft.Ectobi.Common
{
    public class ErrorCodes
    {
        public static readonly int ERR_NOT_FOUND = 0x0001;
        public static readonly int ERR_SCHEMA_ALREADY_EXISTS = 0x0002;
        public static readonly int ERR_ARGUMENT_NULL = 0x0003;
        public static readonly int ERR_REQUIRED_VALUE_EMPTY = 0x0004;
        public static readonly int ERR_UNSPECIFIED_ERROR = 0x0005;
        public static readonly int? ERR_FIELD_ALREADY_EXISTS = 0x0006;
    }
}
