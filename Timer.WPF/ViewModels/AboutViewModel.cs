using Prism.Commands;
using Prism.Events;
using Prism.Services.Dialogs;
using Serilog;
using System;
using System.ComponentModel;
using Timer.Shared.EventAggregatorEvents;

namespace Timer.WPF.ViewModels
{
    public class AboutViewModel : Shared.ViewModels.AboutViewModel, IDialogAware
    {

        // commands
        public DelegateCommand CloseDialogCommand { get; }

        // constructor
        public AboutViewModel(ILogger logger, IEventAggregator eventAggregator) : base(logger)
        {

            // injected
            this.EventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            // set up commands
            this.CloseDialogCommand = new DelegateCommand(() => RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel)));

            // add commands to base collection
            base.AddCommand(this.CloseDialogCommand);

        }

        // IDialogAware implementation
        public event Action<IDialogResult>? RequestClose;

        string IDialogAware.Title => base.Title;

        bool IDialogAware.CanCloseDialog() => true;

        void IDialogAware.OnDialogClosed() => this.PropertyChanged -= this.PropertyChangedEventHandler;

        void IDialogAware.OnDialogOpened(IDialogParameters parameters)
        {

            this.PropertyChanged += this.PropertyChangedEventHandler;

            this.Title = parameters.GetValue<string>(AboutBoxTitleParameterName);
            this.Text = parameters.GetValue<string>(AboutBoxTextParameterName);
            this.SharedVersion = parameters.GetValue<string>(AboutBoxSharedVersionParameterName);
            this.ViewVersion = parameters.GetValue<string>(AboutBoxViewVersionParameterName);
        }

        private void PropertyChangedEventHandler(object? sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName is not null && e.PropertyName.Equals(nameof(base.Text), StringComparison.OrdinalIgnoreCase))
            {
                this.EventAggregator!.GetEvent<ReleaseNotesChangedEvent>().Publish(base.Text);
            }
        }

    }

}
