namespace Trogsoft.Ectobi.Data
{
    public class Record
    {
        public long Id { get; set; }    
        public long BatchId { get; set; }
        public Batch Batch { get; set; }    
        public DateTime Created { get; set; }
        public ICollection<Value> Values { get; set; } = new HashSet<Value>();
    }
}