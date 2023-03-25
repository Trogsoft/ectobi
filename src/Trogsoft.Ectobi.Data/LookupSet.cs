namespace Trogsoft.Ectobi.Data
{
    public class LookupSet : NamedEntity
    {
        public long Id { get; set; }
        public ICollection<LookupSetValue> Values { get; set; } = new HashSet<LookupSetValue>();
    }
}