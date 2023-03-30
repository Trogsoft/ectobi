using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Writers;
using Trogsoft.Ectobi.Common;
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

        public long? GetPopulatorDatabaseId(string populator)
        {
            using (var scope = issf.CreateScope())
            {
                var db = scope.ServiceProvider.GetService<EctoDb>();
                if (db != null)
                    return db.Populators.SingleOrDefault(x => x.Name == populator)?.Id;
            }
            return null;
        }

        public IPopulator GetPopulator(string? name)
        {

            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
            if (!PopulatorExists(name)) throw new Exception($"Populator {name} does not exist.");

            var populatorType = options.Populators.SingleOrDefault(x => x.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
            if (populatorType == null) throw new Exception($"Populator {name} does not exist.");

            using (var scope = issf.CreateScope())
            {
                var instance = (IPopulator)scope.ServiceProvider.GetService(populatorType);
                if (instance == null) throw new Exception($"Unable to create instance of type {name}");
                return instance;
            }

        }

        public Success<List<PopulatorModel>> GetPopulatorDefinitions()
        {

            List<PopulatorModel> pops = new List<PopulatorModel>();
            foreach (var populator in options.Populators)
                pops.Add(new PopulatorModel
                {
                    Name = populator.Name,
                    TextId = populator.FullName ?? populator.Name
                });

            return new Success<List<PopulatorModel>>(pops);

        }
    }
}
