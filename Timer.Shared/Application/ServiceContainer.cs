using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Prism.Ioc;
using Serilog;
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
            containerRegistry.Register<ITimeLogService, Teamwork>();
            containerRegistry.RegisterInstance<ILogger>(seriLog);
            containerRegistry.Register<ISystemClock, SystemClock>();


            containerRegistry.RegisterServices(services =>
            {
                services.AddHttpClient();
            });

        }

    }

}
