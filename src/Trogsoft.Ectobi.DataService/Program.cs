using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Trogsoft.Ectobi.Common.Interfaces;
using Trogsoft.Ectobi.Data;
using Trogsoft.Ectobi.DataService.Interfaces;
using Trogsoft.Ectobi.DataService.Services;
using Microsoft.AspNetCore.Authorization;
using Trogsoft.Ectobi.Common;
using Microsoft.EntityFrameworkCore.Sqlite;
using System.Data;
using Hangfire.LiteDB;
using System.Reflection;

namespace Trogsoft.Ectobi.DataService
{
    public class Program
    {
        private static string? dbType;
        private static bool setupDb;
        private static bool setupMode;
        private static bool forceMode;
        private static bool noSecurityMode;

        public static void Main(string[] args)
        {

            var dbSelector = args.ToList().FindIndex(x => x.Equals("--db", StringComparison.CurrentCultureIgnoreCase));
            if (dbSelector > -1 && args.Count() > dbSelector)
            {
                dbType = args[dbSelector + 1];
            }

            setupDb = args.Contains("--setup-db", StringComparer.CurrentCultureIgnoreCase);
            setupMode = args.Contains("--setup-default", StringComparer.CurrentCultureIgnoreCase);
            forceMode = args.Contains("--force", StringComparer.CurrentCultureIgnoreCase);
            noSecurityMode = args.Contains("--no-security", StringComparer.CurrentCultureIgnoreCase);

            var localConfigPath = new LocalConfigFileProvider();

            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.AddJsonFile(localConfigPath, "local.json", optional: false, reloadOnChange: false);

            var jwtSecret = builder.Configuration.GetSection("Ectobi")?.GetValue<string>("JwtSecret");
            if (jwtSecret == null)
            {
                Console.WriteLine("You must set the JwtSecret configuration value in the following file:");
                Console.WriteLine($"{Path.Combine(localConfigPath.LocalConfigPath, "local.json")}");
                Console.WriteLine();
                Console.WriteLine("Ectobi: { JwtSecret: \"your-secret-key\" }");
                Console.WriteLine();
                return;
            }

            builder.Services.AddDbContext<EctoDb>(db =>
            {
                if (dbType != null)
                {
                    var cs = builder.Configuration.GetConnectionString("db");
                    if (dbType.Equals("local", StringComparison.CurrentCultureIgnoreCase))
                    {
                        var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Ectobi", "Database");
                        Directory.CreateDirectory(path);

                        cs = $"Data Source={Path.Combine(path, "ecto.db")}";
                        db.UseSqlite(cs);
                    }
                }
                else
                {
                    var connectionString = builder.Configuration.GetConnectionString("db");
                    db.UseSqlServer(connectionString);
                }
            });

            // For Identity
            builder.Services.AddIdentity<EctoUser, EctoRole>(opts =>
            {
                if (setupMode)
                {
                    opts.Password.RequireNonAlphanumeric = false;
                    opts.Password.RequireLowercase = false;
                    opts.Password.RequireDigit = false;
                    opts.Password.RequireUppercase = false;
                    opts.Password.RequiredLength = 0;
                }
            })
                .AddEntityFrameworkStores<EctoDb>()
                .AddDefaultTokenProviders();

            // Adding Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })

            // Adding Jwt Bearer
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = "ectobi",
                    ValidIssuer = "ectobi",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
                };
            });

            builder.Services.AddTransient<IEventNotificationService, EventNotificationService>();
            builder.Services.AddTransient<ISchemaService, SchemaService>();
            builder.Services.AddTransient<IFieldService, FieldService>();
            builder.Services.AddTransient<IBatchService, BatchService>();
            builder.Services.AddTransient<ILookupService, LookupService>();
            builder.Services.AddTransient<ILookupStorage, LookupStorage>();
            builder.Services.AddTransient<IWebHookService, WebHookService>();
            builder.Services.AddTransient<IWebHookManagementService, WebHookManagementService>();
            builder.Services.AddTransient<IFileTranslatorService, FileTranslatorService>();
            builder.Services.AddSingleton<IBackgroundTaskCoordinator, BackgroundTaskCoordinator>();
            builder.Services.AddTransient<IAuthService, UserService>();
            builder.Services.AddTransient<IEctoMapper, EctoMapper>();
            builder.Services.AddTransient<IEctoServer, EctoServer>();
            builder.Services.AddTransient<IRandomStringService, RandomStringService>();
            builder.Services.AddTransient<IDataService, Services.DataService>();
            builder.Services.AddTransient<IModelService, ModelService>();

            builder.Services.AddControllers();
            builder.Services.AddSignalR();

            var temporaryStoreProvider = builder.Configuration.GetSection("Ectobi")?.GetValue<string?>("TemporaryStoreProvider");
            if (temporaryStoreProvider == null) temporaryStoreProvider = "SystemTempStore";

            var type = GetLoadedType(temporaryStoreProvider);
            if (type == null) throw new Exception("Temporary store provider could not be found or could not be instantiated.");

            builder.Services.AddTransient(typeof(ITemporaryStore), type);

            var populators = DiscoverModules<IPopulator>(builder.Services);
            var fileHandlers = DiscoverModules<IFileHandler>(builder.Services);
            var models = DiscoverModulesWithAttribute<EctoModelAttribute>(builder.Services);

            builder.Services.AddHttpClient("webhook");

            builder.Services.Configure<ModuleOptions>(opt =>
            {
                opt.Populators.AddRange(populators);
                opt.FileImporters.AddRange(fileHandlers);
                opt.Models.AddRange(models);
            });
            builder.Services.AddSingleton<ModuleManager>();

            if (!setupMode)
            {
                builder.Services.AddHangfire(configuration =>
                {
                    configuration.SetDataCompatibilityLevel(CompatibilityLevel.Version_170);
                    configuration.UseSimpleAssemblyNameTypeSerializer();
                    configuration.UseRecommendedSerializerSettings();
                    if (dbType != null && dbType.Equals("local", StringComparison.CurrentCultureIgnoreCase))
                    {
                        var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Ectobi", "Database");
                        Directory.CreateDirectory(path);
                        var cs = Path.Combine(path, "ecto-hangfire.db");

                        configuration.UseLiteDbStorage(cs, new LiteDbStorageOptions
                        {
                            QueuePollInterval = TimeSpan.FromSeconds(5)
                        });
                    }
                    else
                    {
                        configuration.UseSqlServerStorage(builder.Configuration.GetConnectionString("hangfire"), new SqlServerStorageOptions
                        {
                            CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                            SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                            QueuePollInterval = TimeSpan.Zero,
                            UseRecommendedIsolationLevel = true,
                            DisableGlobalLocks = true
                        });
                    }
                });

                builder.Services.AddHangfireServer();
            }

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var customServerName = builder.Configuration.GetSection("Ectobi")?.GetValue<string>("ServerName");
            builder.Services.Configure<EctoServerModel>(opt =>
            {
                opt.RequiresLogin = !noSecurityMode;
                opt.Name = customServerName ?? Environment.MachineName;
            });

            var app = builder.Build();

            if (setupDb)
            {
                using (var scope = app.Services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetService<EctoDb>();
                    db.Database.EnsureCreated();
                }
            }

            if (setupMode)
                SetupDefault(app);

            app.Services.GetService<ModuleManager>()?.RegisterModules();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                if (!setupMode) app.UseHangfireDashboard();
            }
            else
            {
                app.UseHttpsRedirection();
            }

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapHub<EctoEventHub>("/events");

            if (noSecurityMode)
                app.MapControllers().WithMetadata(new AllowAnonymousAttribute());
            else
                app.MapControllers();

            if (!setupMode)
                app.Run();
            else
                Console.WriteLine("Default setup has completed.");

        }

        private static List<Type> DiscoverModulesWithAttribute<T>(IServiceCollection services) where T: Attribute
        {
            List<Type> modules = new List<Type>();
            foreach (var type in AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes().Where(y => y.IsPublic && !y.IsAbstract && !y.IsInterface && y.GetCustomAttribute<T>() != null)))
            {
                services.AddTransient(type);
                modules.Add(type);
            }
            return modules;
        }

        private static void SetupDefault(WebApplication app)
        {

            using (var scope = app.Services.CreateScope())
            {

                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<EctoUser>>();
                var db = scope.ServiceProvider.GetRequiredService<EctoDb>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                var rm = scope.ServiceProvider.GetRequiredService<RoleManager<EctoRole>>();

                int userCount = db.Users.Count();
                if (userCount > 0 && !forceMode)
                {
                    logger.LogError("Ectobi is already configured. Specify --force to reset to defaults.");
                    return;
                }
                else if (userCount > 0 && forceMode)
                {
                    logger.LogInformation("Resetting to default.");
                    db.Users.RemoveRange(db.Users);
                    db.SaveChanges();
                }

                logger.LogInformation("Creating default administrative user.");

                rm.CreateAsync(new EctoRole { Name = "Administrator" }).Wait();
                rm.CreateAsync(new EctoRole { Name = "SchemaManager" }).Wait();
                rm.CreateAsync(new EctoRole { Name = "FieldManager" }).Wait();
                rm.CreateAsync(new EctoRole { Name = "BatchManager" }).Wait();
                rm.CreateAsync(new EctoRole { Name = "WebHookManager" }).Wait();
                rm.CreateAsync(new EctoRole { Name = "LookupManager" }).Wait();
                rm.CreateAsync(new EctoRole { Name = "FormManager" }).Wait();
                rm.CreateAsync(new EctoRole { Name = "DataEntryOperator" }).Wait();

                var result = userManager.CreateAsync(new EctoUser
                {
                    Email = "ectobi@example.com",
                    EmailConfirmed = true,
                    UserName = "ectobi"
                }, "ectobi")
                .Result;

                if (!result.Succeeded)
                {
                    result.Errors.ToList().ForEach(Console.WriteLine);
                }
                else
                {
                    logger.LogInformation("Adding user to Administrator role.");
                    var user = userManager.FindByNameAsync("ectobi").Result;
                    userManager.AddToRoleAsync(user, "Administrator").Wait();
                }

                logger.LogInformation("Default setup done.");
            }

        }

        private static Type? GetLoadedType(string typeName)
        {
            foreach (var type in AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes().Where(y =>
                    y.Name.Equals(typeName, StringComparison.CurrentCultureIgnoreCase) &&
                    y.IsPublic &&
                    !y.IsAbstract &&
                    !y.IsInterface)))
            {
                return type;
            }
            return null;
        }

        private static List<Type> DiscoverModules<T>(IServiceCollection services)
        {

            List<Type> modules = new List<Type>();
            foreach (var type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes().Where(y => typeof(T).IsAssignableFrom(y) && y.IsPublic && !y.IsAbstract && !y.IsInterface)))
            {
                services.AddTransient(type);
                modules.Add(type);
            }
            return modules;

        }

    }
}