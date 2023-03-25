namespace Trogsoft.Ectobi.Data
{
    public class SchemaVersion
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public int Version { get; set; }
        public DateTime Created { get; set; }
        public string? Description { get; set; }
        public ICollection<SchemaFieldVersion> Fields { get; set; } = new HashSet<SchemaFieldVersion>();
        public long SchemaId { get; set; }
        public Schema Schema {  get; set; }
    }
}