using Trogsoft.Ectobi.Common;

namespace Trogsoft.Ectobi.Data
{
    public class SchemaField : NamedEntity
    {
        public long Id { get; set; }
        public Schema Schema { get; set; } = null!;
        public long SchemaId { get; set; }
        public Schema? ValuesFromSchema { get; set; } 
        public long? ValuesFromSchemaId { get; set; }
        public SchemaFieldType Type { get; set; }
        public SchemaFieldFlags Flags { get; set; }
        public long? PopulatorId { get; set; }
        public Populator? Populator { get; set; }
        public long? ProcessId { get; set; }
        public Process Process { get; set; }
    }
}