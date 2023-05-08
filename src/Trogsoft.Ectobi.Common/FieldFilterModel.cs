namespace Trogsoft.Ectobi.Common
{
    public class FieldFilterModel
    {
        public string? TextId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int DisplayOrder { get; set; }
        public FieldFilterType Type { get; set; }
        public List<FieldFilterOption> Options { get; set; } = new List<FieldFilterOption>();
    }
}