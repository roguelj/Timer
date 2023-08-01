using Prism.Commands;
using Prism.Events;
using Prism.Services.Dialogs;
using Serilog;
using System;
using System.Linq;
using Timer.Shared.Services.Interfaces;
using ResMan = Timer.Shared.Resources.Resources;

namespace Timer.WPF.ViewModels
{
    internal class TimeLogDetailViewModel : TimeLogDetailViewModelBase, IDialogAware
    {

        // commands
        public DelegateCommand CloseDialogOkCommand { get; }
        public DelegateCommand CloseDialogCancelCommand { get; }

        // constructor
        public TimeLogDetailViewModel(ILogger logger, IEventAggregator eventAggregator, ITimeLogService timeLogService) : base(logger, eventAggregator, timeLogService)
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

        async void IDialogAware.OnDialogOpened(IDialogParameters parameters) => await base.Initialise();

        public string Title => ResMan.TimeLogDetailDialogTitle;

    }

}
