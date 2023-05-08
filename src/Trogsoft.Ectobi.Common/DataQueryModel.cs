using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trogsoft.Ectobi.Common
{
    public class DataQueryModel
    {
        public string SchemaTid { get; set; }
        public Dictionary<string, List<string>> Filter { get; set; } = new();
        public int Page { get; set; } = 1;
        public int? RecordsPerPage { get; set; } = 250;

        public void Set(string textId, IEnumerable<object> enumerable) => Filter[textId] = enumerable.Select(x=>x.ToString()).ToList();
        public void Set(string textId, IEnumerable<int> enumerable) => Filter[textId] = enumerable.Select(x => x.ToString()).ToList();
        public List<long> GetNumeric(string key) => Filter.ContainsKey(key) ? Filter[key].Select(x => long.Parse(x)).ToList() : new List<long>();
        public string? First(string key) => Filter.ContainsKey(key) ? Filter[key].FirstOrDefault() : null;
        public int FirstNumeric(string key) => Filter.ContainsKey(key) ? int.Parse(Filter[key].First()) : -1;
    }
}
