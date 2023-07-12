using Microsoft.Extensions.Configuration;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using System.Windows;
using Timer.Shared.ViewModels;
using Timer.WPF.Dialogs;
using Timer.WPF.Shells;
using Timer.WPF.View;
using Timer.WPF.ViewModels;

namespace Timer.WPF
{

    public partial class App : PrismApplication
    {

        private IConfiguration Configuration { get; }

        public App()
        {
            PrismContainerExtension.Init(); // REQUIRED. see https://github.com/dansiegel/Prism.Container.Extensions/issues/80 
            this.Configuration = Shared.Application.ConfigurationServices.GetConfiguration();
        }


        protected override Window CreateShell() => this.Container.Resolve<Shell>();

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {      
            Shared.Application.ServiceContainer.RegisterTypes(containerRegistry, this.Configuration);

            containerRegistry.RegisterDialog<TimeLogDetailDialog, TimeLogDetailViewModel>(Base.TimeLogDialogName);
  
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();

            ViewModelLocationProvider.Register<Shell, ShellViewModel>();
            ViewModelLocationProvider.Register<TimeLogView, TimeLogViewModel>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            base.ConfigureModuleCatalog(moduleCatalog);

            moduleCatalog.AddModule<Timer.Wpf.Modules.TimeLogModule>(InitializationMode.WhenAvailable);

        }

    }

}
