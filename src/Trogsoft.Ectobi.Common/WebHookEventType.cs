using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trogsoft.Ectobi.Common
{
    [Flags]
    public enum WebHookEventType 
    {
        SchemaCreated = 1, 
        SchemaUpdated = 2,
        SchemaDeleted = 4,
        FieldCreated = 8, 
        FieldUpdated = 16, 
        FieldDeleted = 32,
        SchemaVersionCreated = 64,
        SchemaVersionUpdated = 128,
        SchemaVersionDeleted = 256,
        LookupValueCreated = 512,
        LookupValueUpdated = 1024,
        LookupValueDeleted = 2048,
        BatchCreated = 4096,
        BatchUpdated = 8192,
        BatchDeleted = 16384,
        RecordCreated = 32768,
        RecordUpdated = 65535,
        RecordDeleted = 131072,
    }
}
