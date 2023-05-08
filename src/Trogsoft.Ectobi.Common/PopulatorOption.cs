namespace Trogsoft.Ectobi.Common
{
    public class PopulatorOption
    {
        public PopulatorOption(string id, string name, PopulatorOptionType type, string? group = null)
        {
            Id = id;
            Name = name;
            Type = type;
            Group = group;
        }

        public string Id { get; set;  }
        public string Name { get; set;  }
        public PopulatorOptionType Type { get; set; }
        public string? Group { get; }
    }
}