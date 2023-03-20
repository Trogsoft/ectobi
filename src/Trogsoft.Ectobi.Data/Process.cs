namespace Trogsoft.Ectobi.Data
{
    public class Process : NamedEntity
    {
        public long Id { get; set; }
        public ICollection<ProcessElement> Elements { get; set; } = new HashSet<ProcessElement>(); 
    }
}