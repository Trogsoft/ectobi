namespace Trogsoft.Ectobi.Data
{
    public class Period : NamedEntity
    {
        public long Id { get; set; }
        public long SchemaVersionId { get; set; }
        public SchemaVersion SchemaVersion { get; set; }
        public DateTime StartDateUtc { get; set; }
        public  DateTime EndDateUtc { get; set; }
    }
}