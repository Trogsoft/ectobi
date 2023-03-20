using Hangfire;
using Hangfire.SqlServer;
using Microsoft.EntityFrameworkCore;
using Trogsoft.Ectobi.Common.Interfaces;
using Trogsoft.Ectobi.Data;
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
            builder.Services.AddTransient<IBackgroundJobCoordinator, BackgroundJobCoordinator>();
            builder.Services.AddTransient<IEctoMapper, EctoMapper>();
            builder.Services.AddControllers();

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

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseHangfireDashboard();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}