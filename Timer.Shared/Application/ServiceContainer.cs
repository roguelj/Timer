using Azure.Identity;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph;
using Prism.Ioc;
using Serilog;
using Timer.Shared.Models.Options;
using Timer.Shared.Services.Implementations;
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

                containerRegistry.Register<ITimeLogService, Services.PlannerTimeLogService>();
                containerRegistry.RegisterSingleton<AuthService>();

                // register Microsoft Graph client
                containerRegistry.RegisterSingleton<GraphServiceClient>(() =>
                {
                    var graphOptions = configuration.GetSection("Graph");

                    var credential = new ClientSecretCredential(
                        graphOptions["TenantId"],
                        graphOptions["ClientId"],
                        graphOptions["ClientSecret"]);

                    var scopes = new[] { "https://graph.microsoft.com/.default" };

                    return new GraphServiceClient(credential, scopes);
                });

                // register Cosmos client
                containerRegistry.RegisterSingleton<CosmosClient>(() =>
                {
                    var cosmosOptions = configuration.GetSection("Cosmos");

                    return new CosmosClient(cosmosOptions["Endpoint"], cosmosOptions["Key"]);
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
