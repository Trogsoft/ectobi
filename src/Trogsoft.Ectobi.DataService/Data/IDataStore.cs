namespace Trogsoft.Ectobi.DataService.Data
{
    public interface IDataStore
    {
        Task DeleteAllFieldValues(string schemaTid, string fieldTid, int version = 0);
    }
}