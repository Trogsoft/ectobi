using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Writers;
using System.Reflection;
using Trogsoft.Ectobi.Common;
using Trogsoft.Ectobi.Common.Interfaces;
using Trogsoft.Ectobi.Data;
using Trogsoft.Ectobi.Data.Migrations;

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

        public bool FileHandlerExists(string handler)
            => options.FileImporters.Any(x=>x.Name.Equals(handler, StringComparison.CurrentCultureIgnoreCase));

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

        public IFileHandler GetFileHandler(string name) 
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
            if (!FileHandlerExists(name)) throw new Exception($"File Handler {name} does not exist.");

            var handlerType = options.FileImporters.SingleOrDefault(x => x.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
            if (handlerType == null) throw new Exception($"File Handler {name} does not exist.");

            using (var scope = issf.CreateScope())
            {
                var instance = (IFileHandler)scope.ServiceProvider.GetService(handlerType);
                if (instance == null) throw new Exception($"Unable to create instance of type {name}");
                return instance;
            }
        }

        public IFileHandler? GetFileHandlerForFileExtension(string ext)
        {

            foreach (var fi in options.FileImporters)
            {

                var attr = fi.GetCustomAttribute<FileHandlerAttribute>();
                if (attr == null) continue;

                var canHandle = attr.Extensions.Any(x => x.Equals(ext, StringComparison.CurrentCultureIgnoreCase));
                // todo: It should be possible to have multiple handlers and to be able to specify somehow.
                // For now, we return the first thing we come across.

                return GetFileHandler(fi.Name);

            }

            return null;

        }

        public Success<List<FileHandlerModel>> GetFileHandlers()
        {
            List<FileHandlerModel> fileImporterModels = new List<FileHandlerModel>();
            foreach (var fi in options.FileImporters)
            {
                var newItem = new FileHandlerModel
                {
                    Name = fi.Name,
                    TextId = fi.Name
                };

                var attr = fi.GetCustomAttribute<FileHandlerAttribute>();
                if (attr != null)
                {
                    newItem.Name = attr.Name;
                    newItem.Extensions = attr.Extensions;
                }

                fileImporterModels.Add(newItem);
            }
            return new Success<List<FileHandlerModel>>(fileImporterModels);
        }

        public Success<List<PopulatorModel>> GetPopulatorDefinitions()
        {

            List<PopulatorModel> pops = new List<PopulatorModel>();
            foreach (var populator in options.Populators)
                pops.Add(new PopulatorModel
                {
                    Name = populator.Name,
                    TextId = populator.Name 
                });

            return new Success<List<PopulatorModel>>(pops);

        }
    }
}
