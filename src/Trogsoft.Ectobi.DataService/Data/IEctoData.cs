using Microsoft.EntityFrameworkCore.Storage;
using Trogsoft.Ectobi.Common.Interfaces;
using Trogsoft.Ectobi.Data;

namespace Trogsoft.Ectobi.DataService.Data
{
    public interface IEctoData
    {
        ISchemaStore Schema { get; }
        IFieldStore Field { get; }
        EctoDb Store { get; }
        IEctoMapper Mapper { get; }
        ILookupStore Lookup { get; }

        IDbContextTransaction BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();
        bool TransactionStarted();
    }
}
