﻿using Humanizer;
using Microsoft.Extensions.Options;
using Prism.Commands;
using Prism.Events;
using Serilog;
using System.Collections.ObjectModel;
using Timer.Shared.Extensions;
using Timer.Shared.Models.Options;
using Timer.Shared.Models.ProjectManagementSystem.TeamworkV3.Models;
using Timer.Shared.Services.Interfaces;
using Timer.Shared.ViewModels;
using LogResMan = Timer.Shared.Resources.LogMessages;

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


        // -----------------------
        // bound collection properties
        public ObservableCollection<Tag> Tags { get; } = new ObservableCollection<Tag>();
        public ObservableCollection<ProjectTask> Tasks { get; } = new ObservableCollection<ProjectTask>();
        public ObservableCollection<Project> Projects { get; } = new ObservableCollection<Project>();
        public ObservableCollection<Tag> SelectedTags { get; } = new ObservableCollection<Tag>();


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


            // add command to the base list
            base.AddCommand(this.ClearTaskCommand);
            base.AddCommand(this.LoadAllTasksForSelectedProjectCommand);
            base.AddCommand(this.LoadMyTasksForSelectedProjectCommand);
            base.AddCommand(this.SetStartCommand);
            base.AddCommand(this.SetEndCommand);
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

            var endDateLastEntry = (await this.TimeLogService!.GetEndTimeOfLastTimeLogEntryAsync(CancellationToken.None)).Value.DateTime;

            this.StartDateTime = endDateLastEntry;
            this.EndDateTime = this.SystemClock!.UtcNow.DateTime;

        }


        private async Task LoadData()
        {

            // set recent tags
            if (await this.TimeLogService!.RecentTags(CancellationToken.None) is IEnumerable<Tag> recentTags)
            {
                this.Tags.AddRange(recentTags, true);
                this.Logger.Verbose(LogResMan.FoundEntities, recentTags.Count(), "Tag");
            }

            // recent tasks
            if (await this.TimeLogService.RecentTasks(CancellationToken.None) is IEnumerable<ProjectTask> recentTasks)
            {
                this.Tasks.AddRange(recentTasks, true);
                this.Logger.Verbose(LogResMan.FoundEntities, recentTasks.Count(), "Task");
            }

            // recent projects
            if (await this.TimeLogService.RecentProjects(CancellationToken.None) is IEnumerable<Project> recentProjects)
            {
                this.Projects.AddRange(recentProjects, true);
                this.Logger.Verbose(LogResMan.FoundEntities, recentProjects.Count(), "Project");
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
                this.Logger.Verbose(LogResMan.FoundEntities, tags.Count(), "Tag");
            }

            // set all tasks
            if (await this.TimeLogService!.Tasks(CancellationToken.None) is IEnumerable<ProjectTask> tasks)
            {
                this.SelectedTask = null;
                this.Tasks.AddRange(tasks.OrderBy(ob => ob.Name), true);
                this.Logger.Verbose(LogResMan.FoundEntities, tasks.Count(), "Task");
            }

            // set all (or starred only) projects
            if (await this.TimeLogService!.Projects(parameter == "Starred", CancellationToken.None) is IEnumerable<Project> projects)
            {
                this.SelectedProject = null;
                this.Projects.AddRange(projects.OrderBy(ob => ob.Name), true);
                this.Logger.Verbose(LogResMan.FoundEntities, projects.Count(), "Project");
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
                    this.Logger.Verbose(LogResMan.FoundEntities, tasks.Count(), "Task");

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
                    this.Logger.Verbose(LogResMan.FoundEntities, tasks.Count(), "Task");

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

    }

}
