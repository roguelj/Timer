using Humanizer;
using Microsoft.Extensions.Options;
using Prism.Commands;
using Prism.Events;
using Serilog;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using Timer.Shared.Constants;
using Timer.Shared.Extensions;
using Timer.Shared.Models.Options;
using Timer.Shared.Models.ProjectManagementSystem.TeamworkV3.Models;
using Timer.Shared.Services.Interfaces;
using Timer.Shared.ViewModels;

namespace Timer.WPF.ViewModels
{

    public abstract class TimeLogDetailViewModelBase : Base
    {

        // -----------------------
        // member variables
        private DateTime _startDateTime;
        private DateTime _endDateTime;
        private Project? _selectedProject;
        private ProjectTask? _selectedTask;
        private bool _isBillable;
        private string _description = string.Empty;
        private bool _isInitialising;


        // -----------------------
        // bound properties
        public DateTime StartDateTime
        {
            get => this._startDateTime;
            set
            {
                base.SetProperty(ref this._startDateTime, value);
                base.RaisePropertyChanged(nameof(this.Duration));
                base.RaisePropertyChanged(nameof(this.FriendlyDuration));
                base.RaiseCanExecuteChangedForCommandList();
            }
        }

        public DateTime EndDateTime
        {
            get => this._endDateTime;
            set
            {
                base.SetProperty(ref this._endDateTime, value);
                base.RaisePropertyChanged(nameof(this.Duration));
                base.RaisePropertyChanged(nameof(this.FriendlyDuration));
                base.RaiseCanExecuteChangedForCommandList();
            }
        }

        public TimeSpan Duration => this.EndDateTime - this.StartDateTime;

        public string FriendlyDuration => this.Duration.Humanize(2);

        public Project? SelectedProject
        {
            get => this._selectedProject;
            set
            {
                // alert subscribers in the view that the project has changed.
                if (this.SetProperty(ref this._selectedProject, value))
                {
                    this.EventAggregator!.GetEvent<Shared.EventAggregatorEvents.SelectedProjectChangeEvent>().Publish();
                    this.RaiseCanExecuteChangedForCommandList();
                }
            }
        }

        public ProjectTask? SelectedTask
        {
            get => this._selectedTask;
            set
            {
                if(this.SetProperty(ref this._selectedTask, value))
                {
                    this.RaiseCanExecuteChangedForCommandList();
                }
            }
        }

        public bool IsBillable
        {
            get => this._isBillable;
            set => this.SetProperty(ref this._isBillable, value);
        }

        public string Description
        {
            get => this._description;
            set => this.SetProperty(ref this._description, value);
        }

        public bool IsInitialising
        {
            get => this._isInitialising;
            set => this.SetProperty<bool>(ref this._isInitialising, value);
        }


        // -----------------------
        // commands
        public DelegateCommand ClearTaskCommand { get; }
        public DelegateCommand<string> LoadAllCommand { get; }
        public DelegateCommand LoadAllTasksForSelectedProjectCommand { get; }
        public DelegateCommand LoadMyTasksForSelectedProjectCommand { get; }
        public DelegateCommand<int?> SetStartCommand { get; }
        public DelegateCommand<int?> SetEndCommand { get; }
        public DelegateCommand SetStartToEndOfBreakCommand { get; }
        public DelegateCommand SetEndToStartOfBreakCommand { get; }
        public DelegateCommand SetStartToPcBootTimeCommand { get; }


        // -----------------------
        // bound collection properties
        public ObservableCollection<Tag> Tags { get; } = [];
        public ObservableCollection<ProjectTask> Tasks { get; } = [];
        public ObservableCollection<Project> Projects { get; } = [];
        public ObservableCollection<Tag> SelectedTags { get; } = [];


        // -----------------------
        // command gates
        protected bool IsValidForTimeLog(bool? billable)
        {

            if (this.SelectedProject is null)
            {
                return false;
            }
            else if (this.Duration < TimeSpan.FromMinutes(1))
            {
                return false;
            }
            else
            {
                return true;
            }

        }


        // -----------------------
        // injected
        private IOptions<UserInterfaceOptions> Options { get; }


        // -----------------------
        // constructor
        public TimeLogDetailViewModelBase(ILogger logger, IEventAggregator eventAggregator, ITimeLogService timeLogService, ISystemClock systemClock, IOptions<UserInterfaceOptions> options) : base(logger)
        {

            this.IsInitialising = true;

            // register services
            this.EventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
            this.TimeLogService = timeLogService ?? throw new ArgumentNullException(nameof(timeLogService));
            this.SystemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
            this.Options = options ?? throw new ArgumentNullException(nameof(options));


            // commands
            this.ClearTaskCommand = new DelegateCommand(() => this.SelectedTask = null, () => this.SelectedTask is not null);
            this.LoadAllCommand = new DelegateCommand<string>(this.LoadAllDataEventHandler);
            this.LoadAllTasksForSelectedProjectCommand = new DelegateCommand(this.LoadAllTasksForSelectedProject, () => this.SelectedProject is not null);
            this.LoadMyTasksForSelectedProjectCommand = new DelegateCommand(this.LoadMyTasksForSelectedProject, () => this.SelectedProject is not null);
            this.SetStartCommand = new DelegateCommand<int?>(this.SetStart);
            this.SetEndCommand = new DelegateCommand<int?>(this.SetEnd);
            this.SetStartToEndOfBreakCommand = new DelegateCommand(this.SetStartToEndOfBreak);
            this.SetEndToStartOfBreakCommand = new DelegateCommand(this.SetEndToStartOfBreak);
            this.SetStartToPcBootTimeCommand = new DelegateCommand(this.SetStartToPcBootTime);


            // add command to the base list
            base.AddCommand(this.ClearTaskCommand);
            base.AddCommand(this.LoadAllTasksForSelectedProjectCommand);
            base.AddCommand(this.LoadMyTasksForSelectedProjectCommand);
            base.AddCommand(this.SetStartCommand);
            base.AddCommand(this.SetEndCommand);
            base.AddCommand(this.SetStartToEndOfBreakCommand);
            base.AddCommand(this.SetEndToStartOfBreakCommand);
            base.AddCommand(this.SetStartToPcBootTimeCommand);

        }


        protected async Task Initialise()
        {

            // ensure any views that are bound are able to act accordingly
            this.IsInitialising = true;


            // set the start and end dates
            await this.SetDates();


            // load the projects, tasks and tags
            await this.LoadData();


            // ensure any views that are bound are able to act accordingly
            this.IsInitialising = false;

        }


        private async Task SetDates()
        {

            // determine the most sensible start & end dates.
            // get the start date as the time stamp of the last entry end point.
            // the assumption is that the Time Log Entry button is pressed on task finish, so the end date is now

            DateTime endDateLastEntry;

            try
            {
                endDateLastEntry = (await this.TimeLogService!.GetEndTimeOfLastTimeLogEntryAsync(CancellationToken.None)).Value.DateTime;
            }
            catch (Exception ex)
            {
                endDateLastEntry = this.SystemClock!.Now.DateTime;
                this.Logger.Error(ex, LogMessage.EXCEPTION_DURING_METHOD, "b8347e24-25c4-43e4-a9f5-18e663b79e01");
            }


            // set the properties
            this.StartDateTime = endDateLastEntry;
            this.EndDateTime = this.SystemClock!.Now.DateTime;

        }


        private async Task LoadData()
        {

            // set recent tags
            if (await this.TimeLogService!.RecentTags(CancellationToken.None) is IEnumerable<Tag> recentTags)
            {
                this.Tags.AddRange(recentTags, true);
                this.Logger.Information(LogMessage.ENTITY_COUNT, recentTags.Count(), "Tag");
            }
            else
            {
                this.Logger.Warning(LogMessage.RECENT_FAILURE, "Tag");
            }

            // recent tasks
            if (await this.TimeLogService.RecentTasks(CancellationToken.None) is IEnumerable<ProjectTask> recentTasks)
            {
                this.Tasks.AddRange(recentTasks, true);
                this.Logger.Information(LogMessage.ENTITY_COUNT, recentTasks.Count(), "Task");
            }
            else
            {
                this.Logger.Warning(LogMessage.RECENT_FAILURE, "Task");
            }

            // recent projects
            if (await this.TimeLogService.RecentProjects(CancellationToken.None) is IEnumerable<Project> recentProjects)
            {
                this.Projects.AddRange(recentProjects, true);
                this.Logger.Information(LogMessage.ENTITY_COUNT, recentProjects.Count(), "Project");
            }
            else
            {
                this.Logger.Warning(LogMessage.RECENT_FAILURE, "Project");
            }

        }


        private async void LoadAllDataEventHandler(string parameter)
        {

            // ensure any views that are bound are able to act accordingly
            this.IsInitialising = true;


            // set all tags
            if (await this.TimeLogService!.Tags(CancellationToken.None) is IEnumerable<Tag> tags)
            {
                this.SelectedTags.Clear();
                this.Tags.AddRange(tags.OrderBy(ob => ob.Name), true);
                this.Logger.Information(LogMessage.ENTITY_COUNT, tags.Count(), "Tag");
            }
            else
            {
                this.Logger.Warning(LogMessage.ALL_FAILURE, "Tag");
            }


            // set all tasks
            if (await this.TimeLogService!.Tasks(CancellationToken.None) is IEnumerable<ProjectTask> tasks)
            {
                this.SelectedTask = null;
                this.Tasks.AddRange(tasks.OrderBy(ob => ob.Name), true);
                this.Logger.Information(LogMessage.ENTITY_COUNT, tasks.Count(), "Task");
            }
            else
            {
                this.Logger.Warning(LogMessage.ALL_FAILURE, "Task");
            }


            // set all (or starred only) projects
            if (await this.TimeLogService!.Projects(parameter == "Starred", CancellationToken.None) is IEnumerable<Project> projects)
            {
                this.SelectedProject = null;
                this.Projects.AddRange(projects.OrderBy(ob => ob.Name), true);
                this.Logger.Information(LogMessage.ENTITY_COUNT, projects.Count(), "Project");
            }
            else
            {
                this.Logger.Warning(LogMessage.ALL_FAILURE, "Project");
            }


            // ensure any views that are bound are able to act accordingly
            this.IsInitialising = false;

        }


        private async void LoadAllTasksForSelectedProject()
        {

            // load all tasks for selected project
            if(this.SelectedProject is not null)
            {

                if (await this.TimeLogService!.Tasks(this.SelectedProject.Id, CancellationToken.None) is IEnumerable<ProjectTask> tasks)
                {

                    // build a list of tasks that have not already been added to the list.
                    var existingTaskIds = this.Tasks.Select(s => s.Id);
                    var tasksToAdd = tasks.OrderBy(ob => ob.Name).Where(w => !existingTaskIds.Contains(w.Id));


                    // clear current selected task, and update the list
                    // do not clear current list - we're just adding new ones
                    this.SelectedTask = null;
                    this.Tasks.AddRange(tasksToAdd, false);                                 
                    this.Logger.Information(LogMessage.ENTITY_COUNT, tasks.Count(), "Task");

                }

            }  

        }


        private async void LoadMyTasksForSelectedProject()
        {

            // load my tasks for selected project
            if (this.SelectedProject is not null)
            {

                if (await this.TimeLogService!.MyTasks(this.SelectedProject.Id, CancellationToken.None) is IEnumerable<ProjectTask> tasks)
                {

                    // build a list of tasks that have not already been added to the list.
                    var existingTaskIds = this.Tasks.Select(s => s.Id);
                    var tasksToAdd = tasks.OrderBy(ob => ob.Name).Where(w => !existingTaskIds.Contains(w.Id));


                    // clear current selected task, and update the list
                    // do not clear current list - we're just adding new ones
                    this.SelectedTask = null;
                    this.Tasks.AddRange(tasksToAdd, false);
                    this.Logger.Information(LogMessage.ENTITY_COUNT, tasks.Count(), "Task");

                }

            }

        }


        public bool IsTaskOwnedBySelectedProject(ProjectTask? projectTask)
        {
            if(this.SelectedProject is null || projectTask is null) return false;
            return this.SelectedProject.Id == projectTask.ProjectId;
        }

   
        private void SetStart(int? minutes)
        {

            var now = this.SystemClock!.UtcNow;

            if (minutes.HasValue)
            {
                this.StartDateTime = now.AddMinutes(minutes.Value).DateTime;
            }
            else if (this.Options.Value.TimeOfFirstTask is TimeOnly timeOfFirstTask)
            {
                this.StartDateTime = now.Date + timeOfFirstTask.ToTimeSpan();
            }

        }


        private void SetEnd(int? minutes)
        {

            if(minutes.HasValue)
            {
                var now = this.SystemClock!.UtcNow;
                this.EndDateTime = now.AddMinutes(minutes.Value).DateTime;
            }

        }


        private void SetStartToEndOfBreak()
        {

            var now = this.SystemClock!.UtcNow;

            if (this.Options.Value.BreakEnd is TimeOnly breakEnd)
            {
                this.StartDateTime = now.Date + breakEnd.ToTimeSpan();
            }

        }


        private void SetEndToStartOfBreak()
        {

            var now = this.SystemClock!.UtcNow;

            if (this.Options.Value.BreakStart is TimeOnly breakStart)
            {
                this.EndDateTime = now.Date + breakStart.ToTimeSpan();
            }

        }


        private void SetStartToPcBootTime()
        {
            var ticks = Stopwatch.GetTimestamp();
            this.StartDateTime = this.SystemClock!.Now.DateTime.AddSeconds(-(ticks / Stopwatch.Frequency));
        }
  
    }

}
