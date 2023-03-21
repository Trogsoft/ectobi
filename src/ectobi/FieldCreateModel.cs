using Trogsoft.CommandLine;
using Trogsoft.Ectobi.Common;

namespace ectobi
{
    public class FieldCreateModel
    {
        [Parameter('s', "schema", IsRequired = true)]
        public string SchemaTid { get; set; }
        [Parameter('n', "name", IsRequired = true)]
        public string Name { get; set; }

        [Parameter('d', "description", IsRequired = false)]
        public string Description { get; set; }

        [Parameter("type", IsRequired = true, Default = SchemaFieldType.Text)]
        public SchemaFieldType Type { get; set; }

        [Parameter("flags", IsRequired = false, ListSeparator = ",")]
        public SchemaFieldFlags Flags { get; set; }

        [Parameter("populator", IsRequired = false)]
        public string Populator { get; set; }

    }
}