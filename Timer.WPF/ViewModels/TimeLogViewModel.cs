using Prism.Services.Dialogs;
using Serilog;
using System;
using Timer.Shared.Services.Interfaces;
using Timer.Shared.ViewModels;

namespace Timer.WPF.ViewModels
{
    public class TimeLogViewModel:Base
    {


        // injected services
        private IDialogService DialogService { get; }


        // commands
        public Prism.Commands.DelegateCommand LogTimeCommand { get; private set; }
        public Prism.Commands.DelegateCommand OpenSettingsCommand { get; private set; }

        // constructor
        public TimeLogViewModel(ILogger logger, ITimeLogService timeLogService, IDialogService dialogService) :base(logger)
        {

            // initialise services
            this.TimeLogService = timeLogService ?? throw new ArgumentNullException(nameof(timeLogService));
            this.DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));   

            // commands
            this.LogTimeCommand = new Prism.Commands.DelegateCommand(this.LogTimeAsync);
            this.OpenSettingsCommand = new Prism.Commands.DelegateCommand(this.OpenSettings);

        }


        private async void LogTimeAsync()
        {

            // determine the most sensible start & end dates.
            // get the start date as the time stamp of the last entry end point.
            // the assumption is that the Time Log Entry button is pressed on task finish, so the end date is now
            var startDate = await this.TimeLogService!.GetEndTimeOfLastTimeLogEntryAsync();
            var endDate = DateTime.Now;


            // determine the most recent tags, tasks and projects
            var tags = await this.TimeLogService.GetRecentTagsAsync();
            var tasks = await this.TimeLogService.GetRecentTasksAsync();
            var projects = await this.TimeLogService.GetRecentProjectsAsync();


            // create the parameters to pass along to the dialog
            var parameters = new DialogParameters
            {
                { StartTimeDialogParameterName, startDate }, 
                { EndTimeDialogParameterName, endDate},
                { RecentTagsDialogParameterName, tags },
                { RecentTasksDialogParameterName, tasks },
                { RecentProjectsDialogParameterName, projects }
            };


            // open a new dialog with the details
            this.DialogService.ShowDialog(TimeLogDialogName, parameters, LogTimeAsync);

        }

        private void LogTimeAsync(IDialogResult dialogResult) 
        {

            if (dialogResult.Result == ButtonResult.OK)
            {
                
            }

        }

        private void OpenSettings() { }
    }

}