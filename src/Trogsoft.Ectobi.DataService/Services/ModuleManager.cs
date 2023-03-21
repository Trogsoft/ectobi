using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Writers;
using Trogsoft.Ectobi.Common.Interfaces;
using Trogsoft.Ectobi.Data;

namespace Trogsoft.Ectobi.DataService.Services
{
    public class ModuleManager
    {
        private readonly ILogger<ModuleManager> logger;
        private readonly IServiceScopeFactory issf;
        private readonly ModuleOptions options;

        public ModuleManager(ILogger<ModuleManager> logger, IServiceScopeFactory issf, IOptions<ModuleOptions> options)
        {
            this.logger = logger;
            this.issf = issf;
            this.options = options.Value;
        }

        public void RegisterModules()
        {
            using (var scope = issf.CreateScope())
            {
                var db = scope.ServiceProvider.GetService<EctoDb>();
                if (db == null) throw new Exception("Database was null.");

                var populators = 0;
                foreach (var type in options.Populators)
                {
                    if (!typeof(IPopulator).IsAssignableFrom(type)) continue;
                    if (db.Populators.Any(y => y.TextId == type.FullName)) continue;

                    populators++;
                    db.Populators.Add(new Populator
                    {
                        Name = type.Name,
                        TextId = type.FullName
                    });
                }

                db.SaveChanges();

                logger.LogInformation($"Persisted {populators} new populator(s).");

            }
        }

        public bool PopulatorExists(string populator) 
            => options.Populators.Any(x => x.Name.Equals(populator, StringComparison.CurrentCultureIgnoreCase));

        internal long? GetPopulatorDatabaseId(string populator)
        {
            using (var scope = issf.CreateScope())
            {
                var db = scope.ServiceProvider.GetService<EctoDb>();
                if (db != null)
                    return db.Populators.SingleOrDefault(x => x.Name == populator)?.Id;
            }
            return null;
        }
    }
}
