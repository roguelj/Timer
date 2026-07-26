using Azure.Identity;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph;
using Prism.Ioc;
using Serilog;
using Timer.Shared.Models.Options;
using Timer.Shared.Services.Implementations;
using Timer.Shared.Services.Implementations.PlannerAndCosmos;
using Timer.Shared.Services.Interfaces;

namespace Timer.Shared.Application
{
    public static class ServiceContainer
    {

        public static void RegisterTypes(IContainerRegistry containerRegistry, IConfiguration configuration)
        {


            // create a logger instance
            var seriLog = new LoggerConfiguration()
                             .ReadFrom.Configuration(configuration)
                             .CreateLogger();


            // register services

            // register the correct ITimeLogService based on settings
            var timeLogProvider = configuration.GetSection("TimeLogService").Value;
            if (string.Equals(timeLogProvider, "Planner", StringComparison.OrdinalIgnoreCase))
            {

                containerRegistry.Register<ITimeLogService, PlannerTimeLogService>();
                containerRegistry.RegisterSingleton<AuthService>();
                containerRegistry.RegisterSingleton<TimeProvider>(() => TimeProvider.System);

                // register Cosmos client
                containerRegistry.RegisterSingleton<CosmosClient>(() =>
                {

                    var endpoint = configuration["Cosmos:Endpoint"];
                    var key = configuration["Cosmos:Key"];
                         

                    return new CosmosClient(endpoint,  key);
                });
            }
            else
            {
                containerRegistry.Register<ITimeLogService, Services.Implementations.Teamwork.TimeLogService>();
            }

            containerRegistry.RegisterInstance<ILogger>(seriLog);
            containerRegistry.Register<ISystemClock, SystemClock>();


            // register to service collection
            containerRegistry.RegisterServices(services =>
            {
                services.AddHttpClient();
                services.AddMemoryCache();

                services.Configure<TeamworkOptions>(configuration.GetSection("Teamwork"));
                services.Configure<UserInterfaceOptions>(configuration.GetSection("UserInterfaceOptions"));

            });

        }

    }

}
