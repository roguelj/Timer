using Prism.Commands;
using Prism.Events;
using Serilog;
using System.Collections.ObjectModel;
using Timer.Shared.Extensions;
using Timer.Shared.Models;
using Timer.Shared.ViewModels;

namespace Timer.WPF.ViewModels
{
    public abstract class TimeLogDetailViewModelBase : Base
    {

        // member variables
        private DateTime _startDateTime;
        private DateTime _endDateTime;
        private KeyedEntity? _selectedProject;
        private KeyedEntity? _selectedTask;
        private bool _isExtraDetailVisible;
        private string _projectSearchCriteria = string.Empty;
        private string _taskSearchCriteria = string.Empty;
        private string _tagSearchCriteria = string.Empty;
        private bool _isBillable;
        private string _description;


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

        public KeyedEntity? SelectedProject
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

        public KeyedEntity? SelectedTask
        {
            get => this._selectedTask;
            set => this.SetProperty(ref this._selectedTask, value);
        }

        public bool IsExtraDetailVisible
        {
            get => this._isExtraDetailVisible;
            set => this.SetProperty(ref this._isExtraDetailVisible, value);
        }

        public string ProjectSearchCriteria 
        { 
            get => this._projectSearchCriteria;
            set => this.SetProperty(ref this._projectSearchCriteria, value); 
        }

        public string TaskSearchCriteria
        {
            get => this._taskSearchCriteria;
            set => this.SetProperty(ref this._taskSearchCriteria, value);
        }

        public string TagSearchCriteria
        {
            get => this._tagSearchCriteria;
            set => this.SetProperty(ref this._tagSearchCriteria, value);
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


        // commands
        public DelegateCommand ToggleMoreDetailCommand { get; }


        // bound collection properties
        public ObservableCollection<KeyedEntity> Tags { get; } = new ObservableCollection<KeyedEntity>();
        public ObservableCollection<KeyedEntity> Tasks { get; } = new ObservableCollection<KeyedEntity>();
        public ObservableCollection<KeyedEntity> Projects { get; } = new ObservableCollection<KeyedEntity>();
        public ObservableCollection<KeyedEntity> SelectedTags { get; } = new ObservableCollection<KeyedEntity>();
        public ObservableCollection<KeyedEntity> AllProjects { get; } = new ObservableCollection<KeyedEntity>();


        // constructor
        public TimeLogDetailViewModelBase(ILogger logger, IEventAggregator eventAggregator) : base(logger)
        {

            // register services
            this.EventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            // commands
            this.ToggleMoreDetailCommand = new DelegateCommand(() => this.IsExtraDetailVisible = !this.IsExtraDetailVisible);

            // add command to the base list
            base.Commands.Add(this.ToggleMoreDetailCommand);

        }

        protected void Initialise(DateTime startDateTime, DateTime endDateTime, List<KeyedEntity> tags, List<KeyedEntity> tasks, List<KeyedEntity> projects, List<KeyedEntity> allProjects)
        {
            this.StartDateTime = startDateTime;
            this.EndDateTime = endDateTime;
            this.Tags.AddRange(tags);
            this.Tasks.AddRange(tasks);
            this.Projects.AddRange(projects);
            this.AllProjects.AddRange(allProjects);
        }

        public bool IsTaskOwnedBySelectedProject(KeyedEntity? keyedEntity)
        {
            if(this.SelectedProject is null || keyedEntity is null) return false;
            return this.SelectedProject.Id == keyedEntity.ParentId;
        }

        public bool DoesProjectMatchCriteria(KeyedEntity? keyedEntity)
        {
            return keyedEntity?.Name.Contains(this.ProjectSearchCriteria, StringComparison.InvariantCultureIgnoreCase) ?? false;
        }

        public bool DoesTaskMatchCriteria(KeyedEntity? keyedEntity)
        {
            return keyedEntity?.Name.Contains(this.TaskSearchCriteria, StringComparison.InvariantCultureIgnoreCase) ?? false;
        }

        public bool DoesTagMatchCriteria(KeyedEntity? keyedEntity)
        {
            return keyedEntity?.Name.Contains(this.TagSearchCriteria, StringComparison.InvariantCultureIgnoreCase) ?? false;
        }
   
    }

}
