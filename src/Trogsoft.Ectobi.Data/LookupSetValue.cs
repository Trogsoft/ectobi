namespace Trogsoft.Ectobi.Data
{
    public class LookupSetValue
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int NumericValue { get; set; }
        public int Order { get; set; }
        public long LookupSetId { get; set; }
        public LookupSet LookupSet { get; set; }
    }
}