using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using Timer.Shared.Models;
using Timer.WPF.ViewModels;

namespace Timer.WPF.Dialogs
{

    public partial class TimeLogDetailDialog : UserControl
    {
        public TimeLogDetailDialog() => InitializeComponent();

        private void SelectedTagsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            // we need to do this as we cannot bind to the SelectedItems property on the ListBox
            (this.DataContext as TimeLogDetailViewModel)!.SelectedTags.Clear();
            (this.DataContext as TimeLogDetailViewModel)!.SelectedTags.AddRange((sender as ListBox)!.SelectedItems.OfType<KeyedEntity>());

        }

    }

}
