using Trogsoft.Ectobi.Common.Interfaces;

namespace Trogsoft.Ectobi.Data
{
    public class Value : IValueObject
    {
        public long Id { get; set; }
        public long RecordId { get; set; }  
        public Record Record { get; set; }
        public long SchemaFieldVersionId { get; set; }
        public SchemaFieldVersion SchemaFieldVersion { get; set; }
        public string RawValue { get; set; }
        public long? IntegerValue { get; set; }
        public bool? BoolValue { get; set; }
        public double? DecimalValue { get; set; }   
    }
}