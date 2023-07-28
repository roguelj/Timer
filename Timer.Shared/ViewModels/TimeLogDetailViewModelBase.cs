using Prism.Commands;
using Prism.Events;
using Serilog;
using System.Collections.ObjectModel;
using System.Reflection;
using Timer.Shared.Extensions;
using Timer.Shared.Models;
using Timer.Shared.Models.ProjectManagementSystem.TeamworkV3.Models;
using Timer.Shared.ViewModels;

namespace Timer.WPF.ViewModels
{
    public abstract class TimeLogDetailViewModelBase : Base
    {

        // member variables
        private DateTime _startDateTime;
        private DateTime _endDateTime;
        private Shared.Models.ProjectManagementSystem.TeamworkV3.Models.Project? _selectedProject;
        private Shared.Models.ProjectManagementSystem.TeamworkV3.Models.Task? _selectedTask;
        private bool _isExtraDetailVisible;
        private string _projectSearchCriteria = string.Empty;
        private string _taskSearchCriteria = string.Empty;
        private string _tagSearchCriteria = string.Empty;
        private bool _isBillable;
        private string _description = string.Empty;
        private bool _isReady;


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

        public Shared.Models.ProjectManagementSystem.TeamworkV3.Models.Task? SelectedTask
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
            set => this.SetProperty(ref this._isExtraDetailVisible, value);
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

        public bool IsReady
        {
            get => this._isReady;
            set => this.SetProperty<bool>(ref this._isReady, value);
        }

        // commands
        public DelegateCommand ToggleMoreDetailCommand { get; }
        public DelegateCommand ClearTaskCommand { get; }


        // bound collection properties
        public ObservableCollection<Tag> Tags { get; } = new ObservableCollection<Tag>();
        public ObservableCollection<Shared.Models.ProjectManagementSystem.TeamworkV3.Models.Task> Tasks { get; } = new ObservableCollection<Shared.Models.ProjectManagementSystem.TeamworkV3.Models.Task>();
        public ObservableCollection<Project> Projects { get; } = new ObservableCollection<Project>();
        public ObservableCollection<Tag> SelectedTags { get; } = new ObservableCollection<Tag>();
        public ObservableCollection<Project> AllProjects { get; } = new ObservableCollection<Project>();
        public ObservableCollection<Shared.Models.ProjectManagementSystem.TeamworkV3.Models.Task> AllTasks { get; } = new ObservableCollection<Shared.Models.ProjectManagementSystem.TeamworkV3.Models.Task>();
        public ObservableCollection<Tag> AllTags { get; } = new ObservableCollection<Tag>();


        // constructor
        public TimeLogDetailViewModelBase(ILogger logger, IEventAggregator eventAggregator) : base(logger)
        {

            this.IsReady = false;

            // register services
            this.EventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            // commands
            this.ToggleMoreDetailCommand = new DelegateCommand(() => this.IsExtraDetailVisible = !this.IsExtraDetailVisible);
            this.ClearTaskCommand = new DelegateCommand(() => this.SelectedTask = null, () => this.SelectedTask is not null);


            // add command to the base list
            base.Commands.Add(this.ToggleMoreDetailCommand);
            base.Commands.Add(this.ClearTaskCommand);

        }

        protected async System.Threading.Tasks.Task Initialise()
        {

            this.IsReady = false;


            // determine the most sensible start & end dates.
            // get the start date as the time stamp of the last entry end point.
            // the assumption is that the Time Log Entry button is pressed on task finish, so the end date is now
            this.StartDateTime = (await this.TimeLogService!.GetEndTimeOfLastTimeLogEntryAsync(CancellationToken.None)).Value.DateTime;
            this.EndDateTime = DateTime.Now;
            

            // determine the most recent tags, tasks and projects
            var recentProjects = await this.TimeLogService.RecentProjects(CancellationToken.None);
            var recentTasks = await this.TimeLogService.RecentTasks(CancellationToken.None);
            var recentTags = await this.TimeLogService.RecentTags(CancellationToken.None);
   

            //// get all tags, tasks and projects
            //var projects = await this.TimeLogService.Projects(CancellationToken.None);
            //var tasks = await this.TimeLogService.Tasks(CancellationToken.None);
            //var tags = await this.TimeLogService.Tags(CancellationToken.None);
            //var all = new KeyedEntities(projects, tasks, tags.Select(s => s.ToTag()).ToList());

            // log
            //  this.Logger.Verbose(LogResMan.OnDialogOpened, startDateTime, endDateTime, projects?.Count, tasks?.Count, tags?.Count);



            // set recent
            this.Tags.AddRange(recentTags);
            this.Tasks.AddRange(recentTasks);
            this.Projects.AddRange(recentProjects);


            //// set all
            //if(all is not null)
            //{
            //    this.AllProjects.AddRange(all.Projects);
            //    this.AllTasks.AddRange(all.Tasks);
            //    this.AllTags.AddRange(all.Tags);
            //}

            this.IsReady = true;

        }

        public bool IsTaskOwnedBySelectedProject(Shared.Models.ProjectManagementSystem.TeamworkV3.Models.Task? keyedEntity)
        {
            if(this.SelectedProject is null || keyedEntity is null) return false;
            return this.SelectedProject.Id == keyedEntity.ParentId;
        }

        public bool DoesProjectMatchCriteria(Project? keyedEntity)
        {
            return keyedEntity?.Name.Contains(this.ProjectSearchCriteria, StringComparison.InvariantCultureIgnoreCase) ?? false;
        }

        public bool DoesTaskMatchCriteria(Shared.Models.ProjectManagementSystem.TeamworkV3.Models.Task? keyedEntity)
        {
            return keyedEntity?.Name.Contains(this.TaskSearchCriteria, StringComparison.InvariantCultureIgnoreCase) ?? false;
        }

        public bool DoesTagMatchCriteria(Tag? keyedEntity)
        {
            return keyedEntity?.Name.Contains(this.TagSearchCriteria, StringComparison.InvariantCultureIgnoreCase) ?? false;
        }
   
    }

}
