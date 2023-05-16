using Microsoft.EntityFrameworkCore.Storage;
using Trogsoft.Ectobi.Common.Interfaces;
using Trogsoft.Ectobi.Data;
using Trogsoft.Ectobi.DataService.Services;

namespace Trogsoft.Ectobi.DataService.Data;

public class EctoData : IEctoData
{
    private IDbContextTransaction? transaction;

    public Guid InstanceId { get; } = Guid.NewGuid();

    public EctoData(EctoDb db, IWebHookService webHookService, ModuleManager mm, IEctoMapper mapper)
    {
        Store = db;
        WebHooks = webHookService;
        Mapper = mapper;
        ModuleManager = mm;
        Schema = new SchemaStore(this);
        Field = new FieldStore(this);
        Lookup = new LookupStore(this);
        Data = new DataStore(this);
        Progress = new ProgressManager(this);
    }

    public IDbContextTransaction BeginTransaction()
    {
        if (transaction == null)
            transaction = Store.Database.BeginTransaction();
        
        return transaction;
    }

    public bool TransactionStarted() => transaction != null;
    public void CommitTransaction() => transaction?.Commit();
    public void RollbackTransaction() => transaction?.Rollback();

    public EctoDb Store { get; }
    public IWebHookService WebHooks { get; }
    public IEctoMapper Mapper { get; }
    public ModuleManager ModuleManager { get; }
    public ISchemaStore Schema { get; } 
    public IFieldStore Field { get; }
    public ILookupStore Lookup { get; }
    public IDataStore Data { get; }
    public ProgressManager Progress { get; }

}
