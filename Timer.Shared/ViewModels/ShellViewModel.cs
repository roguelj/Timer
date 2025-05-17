using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Timer.Shared.Models.Options;

namespace Timer.Shared.ViewModels
{
    public class ShellViewModel: BaseViewModel
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

        public ShellViewModel(ILogger<ShellViewModel> logger, IOptions<UserInterfaceOptions> options):base(logger)
        {

            // injected
            this.Options = options ?? throw new ArgumentNullException(nameof(options));

            // setup
            this.AlwaysOnTop = options.Value.AlwaysOnTop;

        }

    }

}
