using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trogsoft.Ectobi.Common.Interfaces
{
    public interface ILookupService
    {
        Task<Success<LookupSetModel>> CreateLookupSet(LookupSetModel model);
        Task<Success<LookupSetModel>> GetLookupSet(string lookupTid);
        Task<Success<IEnumerable<LookupSetModel>>> GetLookupSets(string? schemaTid);
    }
}
