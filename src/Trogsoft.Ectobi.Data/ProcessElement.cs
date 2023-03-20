namespace Trogsoft.Ectobi.Data
{
    public class ProcessElement : NamedEntity
    {
        public long Id { get; set; }
        public long ProcessId { get; set; }
        public Process Process { get; set; }
    }
}