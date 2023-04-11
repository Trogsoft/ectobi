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
        public static readonly int ERR_FIELD_ALREADY_EXISTS = 0x0006;
        public static readonly int ERR_BATCH_EXISTS = 0x0007;
        public static readonly int ERR_FILE_NOT_SUPPORTED = 0x0008;
        public static readonly int ERR_LOAD_FILE_FAILED = 0x0009;
        public static readonly int ERR_FILE_PROCESSING_PROBLEM = 0x000a;
        public static readonly int ERR_FILE_NOT_LOADED = 0x000b;
        public static readonly int LOOKUP_SET_CREATION_FAILED = 0x000c;
        public static readonly int ERR_LOGIN_FAILED = 0x000d;
    }
}
