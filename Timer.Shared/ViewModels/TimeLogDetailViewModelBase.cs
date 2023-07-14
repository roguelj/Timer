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


        // bound properties
        public DateTime StartDateTime
        {
            get => this._startDateTime;
            set => this.SetProperty(ref this._startDateTime, value);
        }

        public DateTime EndDateTime
        {
            get => this._endDateTime;
            set => this.SetProperty(ref this._endDateTime, value);
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


        // bound collection properties
        public ObservableCollection<KeyedEntity> Tags { get; } = new ObservableCollection<KeyedEntity>();
        public ObservableCollection<KeyedEntity> Tasks { get; } = new ObservableCollection<KeyedEntity>();
        public ObservableCollection<KeyedEntity> Projects { get; } = new ObservableCollection<KeyedEntity>();
        public ObservableCollection<KeyedEntity> SelectedTags { get; } = new ObservableCollection<KeyedEntity>();


        // constructor
        public TimeLogDetailViewModelBase(ILogger logger, IEventAggregator eventAggregator) : base(logger)
        {

            // register services
            this.EventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

        }

        protected void Initialise(DateTime startDateTime, DateTime endDateTime, List<KeyedEntity> tags, List<KeyedEntity> tasks, List<KeyedEntity> projects)
        {
            this.StartDateTime = startDateTime;
            this.EndDateTime = endDateTime;
            this.Tags.AddRange(tags);
            this.Tasks.AddRange(tasks);
            this.Projects.AddRange(projects);
        }

        public bool IsTaskOwnedBySelectedProject(KeyedEntity? keyedEntityTask)
        {
            if(this.SelectedProject is null || keyedEntityTask is null) return false;
            return this.SelectedProject.Id == keyedEntityTask.ParentId;
        }

    }

}
