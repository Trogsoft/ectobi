using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trogsoft.Ectobi.Common.Interfaces
{

    public interface IEctoModel
    {
        Task<Success<List<EctoModelProperty>>> GetProperties();
    }

    public interface IEctoModel<T> : IEctoModel
    {
        Task<Success<T>> GetPopulatedModel();
    }
}
