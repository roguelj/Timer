using Prism.Services.Dialogs;
using Serilog;
using System;
using Timer.Shared.ViewModels;

namespace Timer.WPF.ViewModels
{
    internal class TimeLogDetailViewModel : Base, IDialogAware
    {
        public TimeLogDetailViewModel(ILogger logger) : base(logger)
        {
        }

        string IDialogAware.Title => "TimeLogDetail";

        public event Action<IDialogResult> RequestClose;

        bool IDialogAware.CanCloseDialog() => true;

        void IDialogAware.OnDialogClosed() { }

        void IDialogAware.OnDialogOpened(IDialogParameters parameters) { }

    }

}
