using Prism.Commands;
using Prism.Services.Dialogs;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Documents;
using Timer.Shared.Models;
using Timer.Shared.ViewModels;
using ResMan = Timer.Shared.Resources.Resources;

namespace Timer.WPF.ViewModels
{
    internal class TimeLogDetailViewModel : Base, IDialogAware
    {
     
        // bound properties
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public ObservableCollection<KeyedEntity> Tags { get; } = new ObservableCollection<KeyedEntity>();
        public ObservableCollection<KeyedEntity> Tasks { get; } = new ObservableCollection<KeyedEntity>();
        public ObservableCollection<KeyedEntity> Projects { get; } = new ObservableCollection<KeyedEntity>();


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
                    { EndTimeDialogParameterName, this.EndDateTime}
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
