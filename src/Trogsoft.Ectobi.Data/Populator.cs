using Trogsoft.Ectobi.Common;

namespace Trogsoft.Ectobi.Data
{
    public class Populator : NamedEntity
    {
        public long Id { get; set; }
        public PopulatorReturnType ReturnType { get; set; }
    }
}