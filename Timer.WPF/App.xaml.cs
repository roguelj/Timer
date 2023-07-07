using Prism.DryIoc;
using Prism.Ioc;
using System;
using System.Windows;

namespace Timer.WPF
{

    public partial class App : PrismApplication
    {
        protected override Window CreateShell() => this.Container.Resolve<MainWindow>();

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            Timer.Shared.PrismSupport.PrismConfig.RegisterTypes(containerRegistry);
        }


    }
}
