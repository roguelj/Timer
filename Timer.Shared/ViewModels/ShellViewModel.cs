using Microsoft.Extensions.Options;
using Serilog;
using Timer.Shared.Models.Options;
using Timer.Shared.Services.Implementations.Auth;

namespace Timer.Shared.ViewModels
{
    public class ShellViewModel:Base
    {

        // member variables
        private bool _alwaysOnTop = true;


        // bound properties
        public bool AlwaysOnTop
        {
            get => this._alwaysOnTop;
            set => this.SetProperty(ref this._alwaysOnTop, value);
        }


        // injected services
        private IOptions<UserInterfaceOptions> Options { get; }
        private AuthService AuthService { get; }

        public ShellViewModel(ILogger logger, IOptions<UserInterfaceOptions> options, AuthService authService):base(logger)
        {

            // injected
            this.Options = options ?? throw new ArgumentNullException(nameof(options));
            this.AuthService = authService ?? throw new ArgumentNullException(nameof(authService));

            // setup
            this.AlwaysOnTop = options.Value.AlwaysOnTop;

        }


    }

}
