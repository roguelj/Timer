using Prism.Commands;
using Prism.Events;
using Prism.Services.Dialogs;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using Timer.Shared.Models;
using ResMan = Timer.Shared.Resources.Resources;
using LogResMan = Timer.Shared.Resources.LogMessages;

namespace Timer.WPF.ViewModels
{
    internal class TimeLogDetailViewModel : TimeLogDetailViewModelBase, IDialogAware
    {

        // commands
        public DelegateCommand CloseDialogOkCommand { get; }
        public DelegateCommand CloseDialogCancelCommand { get; }

        // constructor
        public TimeLogDetailViewModel(ILogger logger, IEventAggregator eventAggregator) : base(logger, eventAggregator)
        {

            // set up commands
            this.CloseDialogOkCommand = new DelegateCommand(this.CloseDialogOk, this.CanCloseDialogOk);
            this.CloseDialogCancelCommand = new DelegateCommand(() => RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel))); // always allow cancel

            this.Commands.Add(this.CloseDialogOkCommand);
            this.Commands.Add(this.CloseDialogCancelCommand);

        }


        // command gates
        private bool CanCloseDialogOk()
        {

            if (this.SelectedProject is null)
            {
                return false;
            }
            else if (this.Duration < TimeSpan.FromSeconds(1))
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        protected virtual void CloseDialogOk()
        {

            var parameters = new DialogParameters
                {
                    { StartTimeDialogParameterName, this.StartDateTime },
                    { EndTimeDialogParameterName, this.EndDateTime},
                    { SelectedProjectDialogParameterName, this.SelectedProject },
                    { SelectedTaskDialogParameterName, this.SelectedTask },
                    { SelectedTagsDialogParameterName, this.SelectedTags.ToList() }
                };

            RequestClose?.Invoke(new DialogResult(ButtonResult.OK, parameters));

        }


        // IDialogAware implementation
        public event Action<IDialogResult>? RequestClose;

        bool IDialogAware.CanCloseDialog() => true;

        void IDialogAware.OnDialogClosed() { }

        void IDialogAware.OnDialogOpened(IDialogParameters parameters)
        {

            // get the parameter values from the dialog parameters
            var startDateTime = parameters.GetValue<DateTime>(StartTimeDialogParameterName);
            var endDateTime = parameters.GetValue<DateTime>(EndTimeDialogParameterName);
            var tags = parameters.GetValue<List<KeyedEntity>>(RecentTagsDialogParameterName);
            var tasks = parameters.GetValue<List<KeyedEntity>>(RecentTasksDialogParameterName);
            var projects = parameters.GetValue<List<KeyedEntity>>(RecentProjectsDialogParameterName);
            var allProjects = parameters.GetValue<List<KeyedEntity>>(AllProjectsDialogParameterName);
            

            // log
            this.Logger.Verbose(LogResMan.OnDialogOpened, startDateTime, endDateTime, projects?.Count, tasks?.Count, tags?.Count);


            // initialise properties on the base class
            base.Initialise(startDateTime, endDateTime, tags, tasks, projects, allProjects);

        }

        public string Title => ResMan.TimeLogDetailDialogTitle;

    }

}
