namespace Trogsoft.Ectobi.Common
{
    public class SchemaFieldModel
    {
        public long Id { get; set; } 
        public string? Name { get; set; } 
        public string? Description { get; set; }
        public string? TextId { get; set; }
        public SchemaFieldType Type { get; set; }
        public SchemaFieldFlags Flags { get; set; }
        public string? Populator { get; set; }
        public string? LookupTid { get; set; }
        public string? ModelName { get; set; }
        public string? ModelField { get; set; }
    }

    public class SchemaFieldEditModel : SchemaFieldModel
    {
        public string? SchemaTid { get; set; }
        public List<string> RawValues { get; set; } = new List<string>();
        public string? ModelField { get; set; }
        public int Version { get; set; }
        public string Set { get; set; }
    }

}