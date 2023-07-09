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


        private async void LogTimeAsync()
        {
            // open a new dialog with the details
            this.DialogService.ShowDialog("TimeLogDetail");
        }

    }

}