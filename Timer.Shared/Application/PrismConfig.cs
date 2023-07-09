using Microsoft.Extensions.Configuration;
using Prism.Ioc;
using Serilog;
using Timer.Shared.Services.Implementations;
using Timer.Shared.Services.Interfaces;

namespace Timer.Shared.PrismSupport
{
    public static class PrismConfig
    {

        public static void RegisterTypes(IContainerRegistry containerRegistry, IConfiguration configuration)
        {

            containerRegistry.Register<ITimeLogService,Teamwork>();

            var seriLog = new LoggerConfiguration()
                             .ReadFrom.Configuration(configuration)
                             .CreateLogger();

            containerRegistry.RegisterInstance<ILogger>(seriLog);

          
        }

    }

}
