using System;
using System.Windows;
using System.Windows.Interop;
using Timer.Shared.Services.Implementations.Auth;

namespace Timer.WPF.Shells
{

    public partial class Shell : Window
    {

        private AuthService AuthService { get; }

        public Shell(AuthService authService)
        {
            this.InitializeComponent();
            this.AuthService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {

            var handle = new WindowInteropHelper(this).Handle;

            await Application.Current.Dispatcher.InvokeAsync(async () => await this.AuthService!.SignIn(handle));

        }

    }

}
