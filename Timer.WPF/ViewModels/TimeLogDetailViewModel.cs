using Prism.Commands;
using Prism.Services.Dialogs;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Timer.Shared.Models;
using Timer.Shared.ViewModels;
using ResMan = Timer.Shared.Resources.Resources;

// TODO: must filter tasks on currently selected project

namespace Timer.WPF.ViewModels
{
    internal class TimeLogDetailViewModel : Base, IDialogAware
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

        public TimeSpan Duration
        {
            get => this.EndDateTime - this.StartDateTime;
        }

        public ObservableCollection<KeyedEntity> Tags { get; } = new ObservableCollection<KeyedEntity>();
        public ObservableCollection<KeyedEntity> Tasks { get; } = new ObservableCollection<KeyedEntity>();
        public ObservableCollection<KeyedEntity> Projects { get; } = new ObservableCollection<KeyedEntity>();
        
        public KeyedEntity SelectedProject
        {
            get => this._selectedProject;
            set => this.SetProperty(ref this._selectedProject, value);
        }

        public KeyedEntity SelectedTask
        {
            get => this._selectedTask;
            set => this.SetProperty(ref this._selectedTask, value);
        }

        public ObservableCollection<KeyedEntity> SelectedTags { get; } = new ObservableCollection<KeyedEntity>();

        public string Title => ResMan.TimeLogDetailDialogTitle;


        // commands
        public DelegateCommand<string> CloseDialogCommand { get; }


        public TimeLogDetailViewModel(ILogger logger) : base(logger)
        {
            this.CloseDialogCommand = new DelegateCommand<string>(this.CloseDialog);
        }

        protected virtual void CloseDialog(string parameter)
        {

            if(bool.TryParse(parameter, out bool res) && res)
            {

                var parameters = new DialogParameters
                {
                    { StartTimeDialogParameterName, this.StartDateTime },
                    { EndTimeDialogParameterName, this.EndDateTime},
                    { SelectedProjectDialogParameterName, this.SelectedProject },
                    { SelectedTaskDialogParameterName, this.SelectedTask },
                    { SelectedTagsDialogParameterName, this.SelectedTags}
                };

                RequestClose?.Invoke(new DialogResult(ButtonResult.OK, parameters));

            }
            else
            {
                RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
            }
                        
        }
                

        public event Action<IDialogResult>? RequestClose;

        bool IDialogAware.CanCloseDialog() => true;

        void IDialogAware.OnDialogClosed() { }

        void IDialogAware.OnDialogOpened(IDialogParameters parameters)
        {

            this.StartDateTime = parameters.GetValue<DateTime>(StartTimeDialogParameterName);
            this.EndDateTime = parameters.GetValue<DateTime>(EndTimeDialogParameterName);
            
            this.Tags.AddRange(parameters.GetValue<List<KeyedEntity>>(RecentTagsDialogParameterName));
            this.Tasks.AddRange(parameters.GetValue<List<KeyedEntity>>(RecentTasksDialogParameterName));
            this.Projects.AddRange(parameters.GetValue<List<KeyedEntity>>(RecentProjectsDialogParameterName));

        }

    }

}
