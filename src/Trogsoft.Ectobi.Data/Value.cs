namespace Trogsoft.Ectobi.Data
{
    public class Value
    {
        public long Id { get; set; }
        public long RecordId { get; set; }  
        public Record Record { get; set; }
        public long SchemaFieldId { get; set; }
        public SchemaField SchemaField { get; set; }
        public string RawValue { get; set; }
        public long? IntegerValue { get; set; }
        public bool? BoolValue { get; set; }
        public double? DecimalValue { get; set; }   
    }
}