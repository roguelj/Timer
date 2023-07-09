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


        // constructor
        public TimeLogViewModel(ILogger logger, ITimeLogService timeLogService, IDialogService dialogService) :base(logger)
        {

            this.TimeLogService = timeLogService ?? throw new ArgumentNullException(nameof(timeLogService));
            this.DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));   

            this.LogTimeCommand = new Prism.Commands.DelegateCommand(this.LogTimeAsync);
        }


        private void LogTimeAsync()
        {

            // determine the most sensible start & end dates
            var startDate = DateTime.Now.AddHours(-1);          // TODO: dummy date. get this from the API
            var endDate = DateTime.Now;                         // TODO: dummy date. get this from the API

            
            // determine the most recent tags, tasks and projects


            // create the parameters to pass along to the dialog
            var parameters = new DialogParameters
            {
                { StartTimeDialogParameterName, startDate }, 
                { EndTimeDialogParameterName, endDate}
            };


            // open a new dialog with the details
            this.DialogService.ShowDialog(TimeLogDialogName, parameters, Callback);

        }

        private void Callback(IDialogResult dialogResult) 
        {

            if (dialogResult.Result == ButtonResult.OK)
            {
                
            }

        }

    }

}