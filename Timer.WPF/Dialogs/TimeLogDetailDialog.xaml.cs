using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using Timer.Shared.Models.ProjectManagementSystem.TeamworkV3.Models;
using Timer.WPF.ViewModels;

namespace Timer.WPF.Dialogs
{

    public partial class TimeLogDetailDialog : UserControl
    {

        // injected services
        private IEventAggregator EventAggregator { get; set; }


        // constructor
        public TimeLogDetailDialog(IEventAggregator eventAggregator)
        {

            this.InitializeComponent();
            this.EventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));


            // subscribe to events. these are raised in the shared viewmodels
            this.EventAggregator.GetEvent<Shared.EventAggregatorEvents.SelectedProjectChangeEvent>().Subscribe(this.ProjectChanged);

        }


        // accessor properties
        private TimeLogDetailViewModel? ViewModel { get => this.DataContext as TimeLogDetailViewModel; }


        // event handlers
        private void SelectedTagsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            // we need to do this as we cannot bind to the SelectedItems property on the ListBox
            (this.DataContext as TimeLogDetailViewModel)!.SelectedTags.Clear();
            (this.DataContext as TimeLogDetailViewModel)!.SelectedTags.AddRange((sender as ListBox)!.SelectedItems.OfType<Tag>());

        }

        private void CollectionViewSource_Filter(object sender, FilterEventArgs e) => e.Accepted = this.ViewModel?.IsTaskOwnedBySelectedProject(e.Item as Shared.Models.ProjectManagementSystem.TeamworkV3.Models.ProjectTask) ?? false;


        // subscribed IEventAggregator event handlers
        private void ProjectChanged() => (this.TaskComboBox.ItemsSource as ListCollectionView)?.Refresh();

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {

            if(e.OriginalSource is Button && e.Source is Xceed.Wpf.Toolkit.DropDownButton source)
            {
                source.IsOpen = false;
            }

        }

    }

}
