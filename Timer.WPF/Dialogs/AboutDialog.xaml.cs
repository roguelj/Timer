using Prism.Events;
using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Timer.Shared.EventAggregatorEvents;

namespace Timer.WPF.Dialogs
{

    public partial class AboutDialog : UserControl
    {

        public AboutDialog(IEventAggregator eventAggregator)
        {
            this.InitializeComponent();
            eventAggregator.GetEvent<ReleaseNotesChangedEvent>().Subscribe(this.ReleaseNotesTextChanged);
        }

        private void ReleaseNotesTextChanged(string text)
        {

            // can't bind to RichTextBox, so we do this.
            var textRange = new TextRange(this.ReleaseNotesFlowDocument.ContentStart, this.ReleaseNotesFlowDocument.ContentEnd);
            using (var sr = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            {
                textRange.Load(sr, DataFormats.Xaml);
            }

        }
    }

}
 