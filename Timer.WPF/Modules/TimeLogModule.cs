using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Timer.WPF.View;

namespace Timer.Wpf.Modules
{
    internal class TimeLogModule : IModule
    {
        void IModule.OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion<TimeLogView>("MainRegion");
        }

        void IModule.RegisterTypes(IContainerRegistry containerRegistry)
        {
   
        }
    }
}
