using Prism.Commands;
using Prism.Events;
using Serilog;
using System.Collections.ObjectModel;
using Timer.Shared.EventAggregatorEvents;
using Timer.Shared.Extensions;
using Timer.Shared.Models.ProjectManagementSystem.TeamworkV3.Models;
using Timer.Shared.Services.Interfaces;
using Timer.Shared.ViewModels;
using LogResMan = Timer.Shared.Resources.LogMessages;

namespace Timer.WPF.ViewModels
{
    public abstract class TimeLogDetailViewModelBase : Base
    {

        // member variables
        private DateTime _startDateTime;
        private DateTime _endDateTime;
        private Project? _selectedProject;
        private ProjectTask? _selectedTask;
        private bool _isExtraDetailVisible;
        private string _projectSearchCriteria = string.Empty;
        private string _taskSearchCriteria = string.Empty;
        private string _tagSearchCriteria = string.Empty;
        private bool _isBillable;
        private string _description = string.Empty;
        private bool _isInitialising;


        // bound properties
        public DateTime StartDateTime
        {
            get => this._startDateTime;
            set
            {
                this.SetProperty(ref this._startDateTime, value);
                this.RaisePropertyChanged(nameof(this.Duration));
            }
        }

        public DateTime EndDateTime
        {
            get => this._endDateTime;
            set
            {
                this.SetProperty(ref this._endDateTime, value);
                this.RaisePropertyChanged(nameof(this.Duration));
            }
        }

        public TimeSpan Duration => this.EndDateTime - this.StartDateTime;

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

        public bool IsExtraDetailVisible
        {
            get => this._isExtraDetailVisible;
            set
            {
                if(this.SetProperty(ref this._isExtraDetailVisible, value) && value)
                {
                    this.EventAggregator!.GetEvent<LoadAllDataRequested>().Publish();
                }
            }
        }

        public string ProjectSearchCriteria 
        { 
            get => this._projectSearchCriteria;
            set
            {
                if(this.SetProperty(ref this._projectSearchCriteria, value))
                {
                    this.EventAggregator!.GetEvent<Shared.EventAggregatorEvents.ProjectSearchCriteriaChangedEvent>().Publish();
                    this.RaiseCanExecuteChangedForCommandList();
                }
            }
        }

        public string TaskSearchCriteria
        {
            get => this._taskSearchCriteria;
            set
            {
                if (this.SetProperty(ref this._taskSearchCriteria, value))
                {
                    this.EventAggregator!.GetEvent<Shared.EventAggregatorEvents.TaskSearchCriteriaChangedEvent>().Publish();
                    this.RaiseCanExecuteChangedForCommandList();
                }
            }
        }

        public string TagSearchCriteria
        {
            get => this._tagSearchCriteria;
            set
            {
                if (this.SetProperty(ref this._tagSearchCriteria, value))
                {
                    this.EventAggregator!.GetEvent<Shared.EventAggregatorEvents.TagSearchCriteriaChangedEvent>().Publish();
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

        // commands
        public DelegateCommand ToggleMoreDetailCommand { get; }
        public DelegateCommand ClearTaskCommand { get; }


        // bound collection properties
        public ObservableCollection<Tag> Tags { get; } = new ObservableCollection<Tag>();
        public ObservableCollection<ProjectTask> Tasks { get; } = new ObservableCollection<ProjectTask>();
        public ObservableCollection<Project> Projects { get; } = new ObservableCollection<Project>();
        public ObservableCollection<Tag> SelectedTags { get; } = new ObservableCollection<Tag>();
        public ObservableCollection<Project> AllProjects { get; } = new ObservableCollection<Project>();
        public ObservableCollection<ProjectTask> AllTasks { get; } = new ObservableCollection<ProjectTask>();
        public ObservableCollection<Tag> AllTags { get; } = new ObservableCollection<Tag>();


        // constructor
        public TimeLogDetailViewModelBase(ILogger logger, IEventAggregator eventAggregator, ITimeLogService timeLogService) : base(logger)
        {

            this.IsInitialising = true;

            // register services
            this.EventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
            this.TimeLogService = timeLogService ?? throw new ArgumentNullException(nameof(timeLogService));


            // commands
            this.ToggleMoreDetailCommand = new DelegateCommand(() => this.IsExtraDetailVisible = !this.IsExtraDetailVisible);
            this.ClearTaskCommand = new DelegateCommand(() => this.SelectedTask = null, () => this.SelectedTask is not null);


            // add command to the base list
            base.Commands.Add(this.ToggleMoreDetailCommand);
            base.Commands.Add(this.ClearTaskCommand);

            this.EventAggregator.GetEvent<LoadAllDataRequested>().Subscribe(async() => await this.LoadAllDataEventHandler());

        }

        protected async Task Initialise()
        {

            // ensure any views that are bound are able to act accordingly
            this.IsInitialising = true;


            // determine the most sensible start & end dates.
            // get the start date as the time stamp of the last entry end point.
            // the assumption is that the Time Log Entry button is pressed on task finish, so the end date is now
            // TODO : if the StartDateTime is the prior day, then it should pick up the start of the current working day. 
            this.StartDateTime = (await this.TimeLogService!.GetEndTimeOfLastTimeLogEntryAsync(CancellationToken.None)).Value.DateTime;
            this.EndDateTime = DateTime.Now;


            // set recent tags
            if(await this.TimeLogService.RecentTags(CancellationToken.None) is IEnumerable<Tag> recentTags)
            {
                this.Tags.AddRange(recentTags, true);
                this.Logger.Verbose(LogResMan.FoundEntities,recentTags.Count(), "Tag");
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


            // ensure any views that are bound are able to act accordingly
            this.IsInitialising = false;

        }


        /// <summary>
        /// Event handler fired when the 'IsExtraDetailVisible' property gets set to true
        /// </summary>
        /// <returns></returns>
        private async Task LoadAllDataEventHandler()
        {

            // ensure any views that are bound are able to act accordingly
            this.IsInitialising = true;


            // set all tags
            if (await this.TimeLogService!.Tags(CancellationToken.None) is IEnumerable<Tag> tags)
            {
                this.AllTags.AddRange(tags, true);
                this.Logger.Verbose(LogResMan.FoundEntities, tags.Count(), "Tag");
            }

            // recent tasks
            if (await this.TimeLogService!.Tasks(CancellationToken.None) is IEnumerable<ProjectTask> tasks)
            {
                this.AllTasks.AddRange(tasks, true);
                this.Logger.Verbose(LogResMan.FoundEntities, tasks.Count(), "Task");
            }

            // recent projects
            if (await this.TimeLogService!.Projects(CancellationToken.None) is IEnumerable<Project> projects)
            {
                this.AllProjects.AddRange(projects, true);
                this.Logger.Verbose(LogResMan.FoundEntities, projects.Count(), "Project");
            }


            // ensure any views that are bound are able to act accordingly
            this.IsInitialising = false;

        }

        /// <summary>
        /// Check whether the supplied task is owned by the currently selected project. Returns false if not, or if there is no selected project, or the selected task is null.
        /// </summary>
        /// <param name="projectTask"></param>
        /// <returns></returns>
        public bool IsTaskOwnedBySelectedProject(ProjectTask? projectTask)
        {
            if(this.SelectedProject is null || projectTask is null) return false;
            return this.SelectedProject.Id == projectTask.ProjectId;
        }

        /// <summary>
        /// Check whether the name of the project contains the search criteria string.
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public bool DoesProjectMatchCriteria(Project? project)
        {
            return project?.Name.Contains(this.ProjectSearchCriteria, StringComparison.InvariantCultureIgnoreCase) ?? false;
        }

        /// <summary>
        /// Check whether the name of the task contains the search criteria string.
        /// </summary>
        /// <param name="projectTask"></param>
        /// <returns></returns>
        public bool DoesTaskMatchCriteria(ProjectTask? projectTask)
        {
            return projectTask?.Name.Contains(this.TaskSearchCriteria, StringComparison.InvariantCultureIgnoreCase) ?? false;
        }

        /// <summary>
        /// Check whether the name of the tag contains the search criteria string.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public bool DoesTagMatchCriteria(Tag? tag)
        {
            return tag?.Name.Contains(this.TagSearchCriteria, StringComparison.InvariantCultureIgnoreCase) ?? false;
        }
   
    }

}
