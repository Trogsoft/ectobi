using Trogsoft.Ectobi.Common;

namespace Trogsoft.Ectobi.Data
{
    public class Batch : NamedEntity
    {
        public long Id { get; set; }
        public string? Source { get; set; }
        public long SchemaVersionId { get; set; }
        public SchemaVersion SchemaVersion { get; set; }
        public DateTime Created { get; set; }
        public BatchFlags Flags { get; set; }
        public ICollection<Record> Records { get; set; } = new HashSet<Record>();
    }
}