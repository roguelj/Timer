using Microsoft.Extensions.Logging;
using Timer.Shared.Services.Interfaces;

namespace Timer.Shared.ViewModels
{
    public class TimeLogViewModel:Base
    {

        // commands
        public Prism.Commands.DelegateCommand LogTimeCommand { get; private set; }

        // constructor
        public TimeLogViewModel(ILogger logger, ITimeLogService timeLogService)
        {

            this.TimeLogService = timeLogService ?? throw new ArgumentNullException(nameof(timeLogService));

            this.LogTimeCommand = new Prism.Commands.DelegateCommand(this.LogTimeAsync);
        }


        private async void LogTimeAsync()
        {

        }

    }

}