namespace Trogsoft.Ectobi.Data
{
    public class Model : NamedEntity
    {
        public long Id { get; set; }
        public string? Handler { get; set; }
        public string? ModelType { get; set; }
    }
}