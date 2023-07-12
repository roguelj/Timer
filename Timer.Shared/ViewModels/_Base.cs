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


        // get the cached type. set it if not already done so
        private Type CachedType => this._cachedType ??= this._cachedType = this.GetType();


        // dialog names
        public const string TimeLogDialogName = "time-log-dialog";


        // dialog parameter names
        protected const string StartTimeDialogParameterName = "start-date-time";
        protected const string EndTimeDialogParameterName = "end-date-time";
        protected const string RecentTagsDialogParameterName = "recent-tags";
        protected const string RecentTasksDialogParameterName = "recent-tasks";
        protected const string RecentProjectsDialogParameterName = "recent-projects";
        protected const string SelectedTaskDialogParameterName = "selected-task";
        protected const string SelectedProjectDialogParameterName = "selected-project";
        protected const string SelectedTagsDialogParameterName = "selected-tags";


        public Base(ILogger logger)
        {

            // services
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // log
            this.Logger.Verbose(LogMessage.TraceMethodHit, "Constructor", this.CachedType.Name);

        }

    }

}

