using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trogsoft.Ectobi.Common
{
    public class ValueMap : Dictionary<string, object>
    {
        public T GetValue<T>(string key, T defaultValue)
        {
            if (this.ContainsKey(key) && typeof(T).IsAssignableFrom(this[key].GetType()))
            {
                return (T)this[key];
            }
            return defaultValue;
        }
    }
}
