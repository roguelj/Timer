using Prism.Ioc;
using Timer.Shared.Services.Implementations;
using Timer.Shared.Services.Interfaces;

namespace Timer.Shared.PrismSupport
{
    public static class PrismConfig
    {

        public static void RegisterTypes(IContainerRegistry containerRegistry)
        {

            containerRegistry.Register<ITimeLogService,DummyTimeLog>();
        }

    }

}
