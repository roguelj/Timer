using Prism.Mvvm;
using Serilog;
using Timer.Shared.Services.Interfaces;
using LogMessage = Timer.Shared.Resources.LogMessages;

namespace Timer.Shared.ViewModels
{
    public class Base:BindableBase
    {

        // member variables
        private Type? _cachedType;

        // injected services
        public ILogger Logger { get; }
        public ITimeLogService? TimeLogService { get; set; }

        private Type CachedType => this._cachedType ??= this._cachedType = this.GetType();

        public Base(ILogger logger)
        {

            // services
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // log
            this.Logger.Verbose(LogMessage.TraceMethodHit, "Constructor", this.CachedType.Name);

        }

    }

}

