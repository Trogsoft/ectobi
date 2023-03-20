namespace Trogsoft.Ectobi.Data
{
    public class Batch : NamedEntity
    {
        public long Id { get; set; }
        public string? Source { get; set; }
        public long SchemaId { get; set; }
        public Schema Schema { get; set; }
        public DateTime Created { get; set; }
    }
}