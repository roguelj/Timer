﻿using Prism.Commands;
using Prism.Events;
using Serilog;
using System.Collections.ObjectModel;
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
        private KeyedEntity? _selectedProject;
        private KeyedEntity? _selectedTask;
        private bool _isExtraDetailVisible;
        private string _projectSearchCriteria = string.Empty;
        private string _taskSearchCriteria = string.Empty;
        private string _tagSearchCriteria = string.Empty;
        private bool _isBillable;
        private string _description = string.Empty;


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


        // commands
        public DelegateCommand ToggleMoreDetailCommand { get; }
        public DelegateCommand ClearTaskCommand { get; }


        // bound collection properties
        public ObservableCollection<Tag> Tags { get; } = new ObservableCollection<Tag>();
        public ObservableCollection<KeyedEntity> Tasks { get; } = new ObservableCollection<KeyedEntity>();
        public ObservableCollection<KeyedEntity> Projects { get; } = new ObservableCollection<KeyedEntity>();
        public ObservableCollection<KeyedEntity> SelectedTags { get; } = new ObservableCollection<KeyedEntity>();
        public ObservableCollection<KeyedEntity> AllProjects { get; } = new ObservableCollection<KeyedEntity>();
        public ObservableCollection<KeyedEntity> AllTasks { get; } = new ObservableCollection<KeyedEntity>();
        public ObservableCollection<Tag> AllTags { get; } = new ObservableCollection<Tag>();


        // constructor
        public TimeLogDetailViewModelBase(ILogger logger, IEventAggregator eventAggregator) : base(logger)
        {

            // register services
            this.EventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            // commands
            this.ToggleMoreDetailCommand = new DelegateCommand(() => this.IsExtraDetailVisible = !this.IsExtraDetailVisible);
            this.ClearTaskCommand = new DelegateCommand(() => this.SelectedTask = null, () => this.SelectedTask is not null);


            // add command to the base list
            base.Commands.Add(this.ToggleMoreDetailCommand);
            base.Commands.Add(this.ClearTaskCommand);

        }

        protected void Initialise(DateTime startDateTime, DateTime endDateTime, KeyedEntities recent, KeyedEntities all)
        {
            this.StartDateTime = startDateTime;
            this.EndDateTime = endDateTime;

            // set recent
            this.Tags.AddRange(recent.Tags);
            this.Tasks.AddRange(recent.Tasks);
            this.Projects.AddRange(recent.Projects);

            // set all
            this.AllProjects.AddRange(all.Projects);
            this.AllTasks.AddRange(all.Tasks);
            this.AllTags.AddRange(all.Tags);

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

        public bool DoesTagMatchCriteria(Tag? keyedEntity)
        {
            return keyedEntity?.Name.Contains(this.TagSearchCriteria, StringComparison.InvariantCultureIgnoreCase) ?? false;
        }
   
    }

}
