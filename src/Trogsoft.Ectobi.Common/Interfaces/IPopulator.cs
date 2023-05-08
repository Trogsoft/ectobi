using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trogsoft.Ectobi.Common.Interfaces
{
    public interface IPopulator
    {
        string GetString(Dictionary<string, string> options);
        long GetInteger(Dictionary<string, string> options);
        double GetDecimal(Dictionary<string, string> options);
        IList<PopulatorOption> GetOptions();
        PopulatorReturnType GetReturnType();
    }
}
