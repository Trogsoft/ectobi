using Hangfire;
using Hangfire.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System.Runtime.CompilerServices;
using Trogsoft.Ectobi.Common.Interfaces;
using Trogsoft.Ectobi.Data;
using Trogsoft.Ectobi.DataService.Interfaces;
using Trogsoft.Ectobi.DataService.Services;

namespace Trogsoft.Ectobi.DataService
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var localConfigPath = new LocalConfigFileProvider();

            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.AddJsonFile(localConfigPath, "local.json", optional: false, reloadOnChange: false);

            builder.Services.AddDbContext<EctoDb>(db =>
            {
                var connectionString = builder.Configuration.GetConnectionString("db");
                db.UseSqlServer(connectionString);
            });

            builder.Services.AddTransient<IEventNotificationService, EventNotificationService>();
            builder.Services.AddTransient<ISchemaService, SchemaService>();
            builder.Services.AddTransient<IFieldService, FieldService>();
            builder.Services.AddTransient<IBatchService, BatchService>();
            builder.Services.AddTransient<ILookupService, LookupService>();
            builder.Services.AddTransient<ILookupStorage, LookupStorage>();
            builder.Services.AddTransient<IWebHookService, WebHookService>();
            builder.Services.AddTransient<IWebHookManagementService,  WebHookManagementService>();
            builder.Services.AddTransient<IFileTranslatorService, FileTranslatorService>();
            builder.Services.AddSingleton<IBackgroundTaskCoordinator, BackgroundTaskCoordinator>();
            builder.Services.AddTransient<IEctoMapper, EctoMapper>();
            builder.Services.AddControllers();

            builder.Services.AddSignalR();

            var populators = DiscoverModules<IPopulator>(builder.Services);
            var fileHandlers = DiscoverModules<IFileHandler>(builder.Services);

            builder.Services.AddHttpClient("webhook");

            builder.Services.Configure<ModuleOptions>(opt => opt.Populators.AddRange(populators));
            builder.Services.Configure<ModuleOptions>(opt => opt.FileImporters.AddRange(fileHandlers));
            builder.Services.AddSingleton<ModuleManager>();

            builder.Services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(builder.Configuration.GetConnectionString("hangfire"), new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                }));

            builder.Services.AddHangfireServer();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.Services.GetService<ModuleManager>()?.RegisterModules();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseHangfireDashboard();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapHub<EventHub>("/events");
            app.MapControllers();

            app.Run();
        }

        private static List<Type> DiscoverModules<T>(IServiceCollection services)
        {

            List<Type> modules = new List<Type>();
            foreach (var type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(x=>x.GetTypes().Where(y=>typeof(T).IsAssignableFrom(y) && y.IsPublic && !y.IsAbstract && !y.IsInterface)))
            {
                services.AddTransient(type);
                modules.Add(type);
            }
            return modules;

        }

    }
}