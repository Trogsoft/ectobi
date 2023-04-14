using Trogsoft.Ectobi.Common.Interfaces;

namespace Trogsoft.Ectobi.DataService
{
    public class ModuleOptions
    {
        public List<Type> Populators { get; set; } = new List<Type>();
        public List<Type> FileImporters { get; set; } = new List<Type>();
        public List<Type> Models { get; set; } = new List<Type>();
    }
}
