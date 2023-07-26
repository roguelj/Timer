using Prism.Events;
using Prism.Mvvm;
using Serilog;
using System.Runtime.CompilerServices;
using Timer.Shared.Services.Interfaces;
using LogMessage = Timer.Shared.Resources.LogMessages;

namespace Timer.Shared.ViewModels
{
    public abstract class Base : BindableBase
    {

        // member variables
        private Type? _cachedType;


        // injected services
        protected ILogger Logger { get; }
        protected ITimeLogService? TimeLogService { get; set; }
        protected IEventAggregator? EventAggregator { get; set; }


        // get the cached type. set it if not already done so
        private Type CachedType => this._cachedType ??= this._cachedType = this.GetType();


        // dialog names
        public const string TimeLogDialogName = "time-log-dialog";
        public const string AboutBoxDialogName = "about-box-dialog";


        // dialog parameter names
        protected const string StartTimeDialogParameterName = "start-date-time";
        protected const string EndTimeDialogParameterName = "end-date-time";
        protected const string RecentTagsDialogParameterName = "recent-tags";
        protected const string RecentTasksDialogParameterName = "recent-tasks";
        protected const string RecentProjectsDialogParameterName = "recent-projects";
        protected const string SelectedTaskDialogParameterName = "selected-task";
        protected const string SelectedProjectDialogParameterName = "selected-project";
        protected const string SelectedTagsDialogParameterName = "selected-tags";
        protected const string ProjectsDialogParameterName = "all-projects";
        protected const string TasksDialogParameterName = "all-tasks";
        protected const string TagsDialogParameterName = "all-tags";
        protected const string IsBillableDialogParameterName = "is-billable";
        protected const string DescriptionDialogParameterName = "description";
        protected const string AboutBoxTitleParameterName = "about-title";                      // the title of the about box
        protected const string AboutBoxTextParameterName = "about-text";                        // the text of the about box
        protected const string AboutBoxSharedVersionParameterName = "about-shared-version";     // the version of the about box for the shared assembly
        protected const string AboutBoxViewVersionParameterName = "about-view-version";         // the version of the about box for the view assembly



        protected List<Prism.Commands.DelegateCommand> Commands { get; } = new List<Prism.Commands.DelegateCommand>();


        public Base(ILogger logger)
        {

            // services
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // log
            this.Logger.Verbose(LogMessage.TraceMethodHit, "Constructor", this.CachedType.Name);

        }

        public Base(ILogger logger, ITimeLogService timeLogService)
        {

            // services
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.TimeLogService = timeLogService ?? throw new ArgumentNullException(nameof(timeLogService));

            // log
            this.Logger.Verbose(LogMessage.TraceMethodHit, "Constructor", this.CachedType.Name);

        }


        protected void RaiseCanExecuteChangedForCommandList()
        {
            foreach(var command in Commands)
            {
                command.RaiseCanExecuteChanged();
            }
        }


        // provide logging for the SetProperty method
        protected override bool SetProperty<T>(ref T storage, T value, Action onChanged, [CallerMemberName] string propertyName = null)
        {
            return base.SetProperty(ref storage, value, () => this.Logger.Verbose(LogMessage.PropertySet, value, propertyName), propertyName);
        }
    
    }

}

