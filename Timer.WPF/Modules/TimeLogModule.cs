using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Timer.WPF.View;

namespace Timer.Wpf.Modules
{
    internal class TimeLogModule : IModule
    {
        void IModule.OnInitialized(IContainerProvider containerProvider) => containerProvider.Resolve<IRegionManager>().RegisterViewWithRegion<TimeLogView>("MainRegion");

        void IModule.RegisterTypes(IContainerRegistry containerRegistry) { }
    }
}
