using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using System.Linq;
using System;
using System.Windows;
using Timer.Shared.Application;
using Timer.Shared.Models.Options;
using Timer.Shared.ViewModels;
using Timer.WPF.Dialogs;
using Timer.WPF.Shells;
using Timer.WPF.View;
using Timer.WPF.ViewModels;
using AboutViewModel = Timer.WPF.ViewModels.AboutViewModel;

namespace Timer.WPF
{

    public partial class App : PrismApplication
    {

        private IConfiguration Configuration { get; }

        public App()
        { 
            PrismContainerExtension.Init();     // REQUIRED. see https://github.com/dansiegel/Prism.Container.Extensions/issues/80 
            this.Configuration = ConfigurationServices.GetConfiguration();
        }


        protected override Window CreateShell()
        {

            var options = this.Container.Resolve<IOptions<UserInterfaceOptions>>();


            // set theme
            var theme = options.Value.Theme;


            // don't allow the user to inject anything they want.
            var allowedthemes = new[] { "Light", "Dark" };


            // do some sanity checks, and clear / re-add the ResourceDictionary's to the MergedDictionaries
            if (theme is not null && !string.IsNullOrEmpty(theme) && allowedthemes.Contains(theme))
            {
                var md = Application.Current.Resources.MergedDictionaries;
                md.Clear();

                md.Add(new ResourceDictionary()
                {
                    Source = new Uri($"/Styles/{theme}/ColourDictionary.xaml", UriKind.RelativeOrAbsolute)
                });

                md.Add(new ResourceDictionary()
                {
                    Source = new Uri("/Styles/Common/Templates.xaml", UriKind.RelativeOrAbsolute)
                });

                md.Add(new ResourceDictionary()
                {
                    Source = new Uri("/Styles/Common/ControlStyles.xaml", UriKind.RelativeOrAbsolute)
                });

            }


            return this.Container.Resolve<Shell>();

        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {      

            // register types in the shared namespace
            ServiceContainer.RegisterTypes(containerRegistry, this.Configuration);

            // register dialogs
            containerRegistry.RegisterDialog<TimeLogDetailDialog, TimeLogDetailViewModel>(Base.TimeLogDialogName);
            containerRegistry.RegisterDialog<AboutDialog, AboutViewModel> (Base.AboutBoxDialogName);
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
