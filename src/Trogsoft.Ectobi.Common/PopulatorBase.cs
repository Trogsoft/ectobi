using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Trogsoft.Ectobi.Common.Interfaces;

namespace Trogsoft.Ectobi.Common
{
    public abstract class PopulatorBase : IPopulator
    {
        public virtual double GetDecimal(Dictionary<string, string> options) => 0d;
        public virtual long GetInteger(Dictionary<string, string> options) => 0;
        public virtual IList<PopulatorOption> GetOptions() => new List<PopulatorOption>();
        public virtual string GetString(Dictionary<string, string> options) => String.Empty;

        public PopulatorReturnType GetReturnType()
        {
            var attr = this.GetType().GetCustomAttribute<PopulatorReturnTypeAttribute>();
            if (attr != null)
                return attr.Type;
            return PopulatorReturnType.String;
        }

    }
}
