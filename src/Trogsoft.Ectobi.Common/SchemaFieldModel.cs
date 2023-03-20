namespace Trogsoft.Ectobi.Common
{
    public class SchemaFieldModel
    {
        public long Id { get; set; } 
        public string? Name { get; set; } 
        public string? Description { get; set; }
        public string? TextId { get; set; }
        public long? ValuesFromSchemaId { get; set; } = null;
        public SchemaFieldType Type { get; set; }
        public SchemaFieldFlags Flags { get; set; }
        public string? Populator { get; set; }
    }

    public class SchemaFieldEditModel : SchemaFieldModel
    {
        public List<string> RawValues { get; set; } = new List<string>();

    }

}