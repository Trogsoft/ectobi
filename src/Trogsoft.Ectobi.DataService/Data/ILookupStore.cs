using Trogsoft.Ectobi.Common;

namespace Trogsoft.Ectobi.DataService.Data
{
    public interface ILookupStore
    {
        Task<LookupSetModel> CreateLookupSet(LookupSetModel model);
        Task<bool> DeleteLookupSet(string tid);
        Task<IEnumerable<LookupSetModel>> FindMatchingLookupSets(LookupSetModel model);
        Task<LookupSetModel> GetLookupSet(string tid);
        Task<long> GetLookupSetId(string tid);
        Task<IEnumerable<LookupSetModel>> GetLookupSets(string? schemaTid = null);
        Task<bool> LookupSetExists(string tid);
    }
}