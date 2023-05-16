using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trogsoft.Ectobi.Common.Interfaces
{
    public interface IScriptingService
    {
        IScriptingFormulaResult ExecuteFormula(string formula, Dictionary<string, object> values);
    }
}
