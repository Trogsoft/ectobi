namespace Trogsoft.Ectobi.Data
{
    public class Schema : NamedEntity
    {
        public long Id { get; set; }
        public ICollection<SchemaVersion> Versions { get; set; } = new HashSet<SchemaVersion>();    
        public ICollection<SchemaField> SchemaFields { get; set; } = new HashSet<SchemaField>();
        public ICollection<Batch> Batches { get; set; } = new HashSet<Batch>();

    }
}