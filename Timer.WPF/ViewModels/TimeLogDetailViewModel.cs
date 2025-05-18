using Microsoft.Extensions.Options;
using Prism.Commands;
using Prism.Events;
using Prism.Services.Dialogs;
using System;
using System.Linq;
using Timer.Shared.Models.Options;
using Timer.Base.Interfaces;
using ResMan = Timer.Shared.Resources.Resources;
using Microsoft.Extensions.Logging;

namespace Timer.WPF.ViewModels
{
    internal class TimeLogDetailViewModel : TimeLogDetailViewModelBase, IDialogAware
    {

        // commands
        public DelegateCommand<bool?> CloseDialogOkCommand { get; }
        public DelegateCommand CloseDialogCancelCommand { get; }


        // constructor
        public TimeLogDetailViewModel(ILogger<TimeLogDetailViewModel> logger, IEventAggregator eventAggregator, ITimeLogService timeLogService, ISystemClock systemClock, IOptions<UserInterfaceOptions> options) 
            : base(logger, eventAggregator, timeLogService, systemClock, options)
        {

            // set up commands
            this.CloseDialogOkCommand = new DelegateCommand<bool?>(this.CloseDialogOk, this.IsValidForTimeLog);
            this.CloseDialogCancelCommand = new DelegateCommand(() => RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel))); // always allow cancel
            
            this.AddCommand(this.CloseDialogOkCommand);
            this.AddCommand(this.CloseDialogCancelCommand);

        }


        protected virtual void CloseDialogOk(bool? billable)
        {

            var parameters = new DialogParameters
            {
                { StartTimeDialogParameterName, this.StartDateTime },
                { EndTimeDialogParameterName, this.EndDateTime},
                { SelectedProjectDialogParameterName, this.SelectedProject },
                { SelectedTaskDialogParameterName, this.SelectedTask },
                { SelectedTagsDialogParameterName, this.SelectedTags.ToList() },
                { IsBillableDialogParameterName, billable },
                { DescriptionDialogParameterName, this.Description },
            };

            RequestClose?.Invoke(new DialogResult(ButtonResult.OK, parameters));

        }



        // IDialogAware implementation
        public event Action<IDialogResult>? RequestClose;

        bool IDialogAware.CanCloseDialog() => true;

        void IDialogAware.OnDialogClosed() { }

        async void IDialogAware.OnDialogOpened(IDialogParameters parameters) => await base.Initialise();

        public string Title => ResMan.TimeLogDetailDialogTitle;

    }

}
