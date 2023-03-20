using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trogsoft.Ectobi.Common.Interfaces
{
    public interface IEctoMapper
    {
        TDest Map<TDest>(object source);
        TDest Map<TSource, TDest>(TSource source);
    }
}
