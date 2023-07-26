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
            else if (this.Duration < TimeSpan.FromMinutes(1))
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
                { SelectedTagsDialogParameterName, this.SelectedTags.ToList() },
                { IsBillableDialogParameterName, this.IsBillable },
                { DescriptionDialogParameterName, this.Description },
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

            // get recent
            var recentTags = parameters.GetValue<List<KeyedEntity>>(RecentTagsDialogParameterName);
            var recentTasks = parameters.GetValue<List<KeyedEntity>>(RecentTasksDialogParameterName);
            var recentProjects = parameters.GetValue<List<KeyedEntity>>(RecentProjectsDialogParameterName);

            // get all
            var allTags = recentTags;           // TODO: actually get all tags
            var allTasks = recentTasks;         // TODO: actually get all tasks
            var allProjects = parameters.GetValue<List<KeyedEntity>>(AllProjectsDialogParameterName);
            

            // log
            this.Logger.Verbose(LogResMan.OnDialogOpened, startDateTime, endDateTime, recentProjects?.Count, recentTasks?.Count, recentTags?.Count);


            // initialise properties on the base class
            var recent = new KeyedEntities(recentProjects, recentTasks, recentTags);
            var all = new KeyedEntities(allProjects, allTasks, allTags);

            base.Initialise(startDateTime, endDateTime, recent, all);

        }

        public string Title => ResMan.TimeLogDetailDialogTitle;

    }

}
