using Trogsoft.Ectobi.Common;

namespace Trogsoft.Ectobi.Data
{
    public class SchemaFieldVersion : NamedEntity
    {
        public long Id { get; set; }
        public long SchemaVersionId { get; set; }
        public SchemaVersion? SchemaVersion { get; set; }
        public long SchemaFieldId { get; set; }
        public SchemaField SchemaField { get; set; }
        public SchemaFieldType Type { get; set; }
        public SchemaFieldFlags Flags { get; set; }
        public long? PopulatorId { get; set; }
        public Populator? Populator { get; set; }
        public long? ProcessId { get; set; }
        public Process? Process { get; set; }
        public long? LookupSetId { get; set; }
        public LookupSet? LookupSet {  get; set; }
        public int DisplayOrder { get; set; }
        public long? ModelId { get; set; }
        public Model? Model { get; set; }
        public string? ModelField { get; set; }

    }
}