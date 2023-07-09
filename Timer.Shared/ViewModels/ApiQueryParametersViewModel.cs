using Microsoft.AspNetCore.Authentication;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using Timer.Shared.Extensions;
using Timer.Shared.Models.ProjectManagementSystem.TeamworkV3;

namespace Timer.Shared.ViewModels
{
    public class ApiQueryParameters : BindableBase
    {


        // ---------------------------------
        // member variables
        private DateTime _from;
        private DateTime _to;
        private bool _isTimeTagsEnabled;
        private bool _isProjectTagsEnabled;


        private ISystemClock Clock { get; set; }


        // ---------------------------------
        // bound properties
        public DateTime From
        {
            get => this._from;
            set => this.SetProperty(ref _from, value);
        }

        public DateTime To
        {
            get => this._to;
            set => this.SetProperty(ref _to, value);
        }

        public bool IsTimeTagsEnabled
        {
            get => this._isTimeTagsEnabled;
            set => this.SetProperty(ref _isTimeTagsEnabled, value);
        }

        public bool IsProjectTagsEnabled
        {
            get => this._isProjectTagsEnabled;
            set => this.SetProperty(ref _isProjectTagsEnabled, value);
        }

        // ---------------------------------
        // observable collections
        public ObservableCollection<User> Users { get; set; }
        public ObservableCollection<Project> Projects { get; set; }
        public ObservableCollection<Models.ProjectManagementSystem.TeamworkV3.Tag> ProjectTags { get; set; }
        public ObservableCollection<Models.ProjectManagementSystem.TeamworkV3.Tag> TaskTags { get; set; }
        public ObservableCollection<Models.ProjectManagementSystem.TeamworkV3.Tag> TimeTags { get; set; }


        // ---------------------------------
        // bound selections
        public object SelectedUsers { get; set; }

        public object SelectedProjects { get; set; }

        public object SelectedProjectTags { get; set; }

        public object SelectedTaskTags { get; set; }

        public object SelectedTimeTags { get; set; }


        public Prism.Commands.DelegateCommand<string> SetDatesCommand { get; private set; }


        // ---------------------------------
        // constructor
        public ApiQueryParameters(ISystemClock systemClock, bool isTimeTagsEnabled, bool isProjectTagsEnabled)
        {

            // injected dependencies
            this.Clock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));


            // initialise collections
            this.Users = new ObservableCollection<User>();
            this.Projects = new ObservableCollection<Project>();
            this.ProjectTags = new ObservableCollection<Tag>();
            this.TaskTags = new ObservableCollection<Tag>();
            this.TimeTags = new ObservableCollection<Tag>();

            // initialise selected objects
            this.SelectedUsers = new object();
            this.SelectedProjects = new object();
            this.SelectedProjectTags = new object();
            this.SelectedTaskTags = new object();
            this.SelectedTimeTags = new object();


            // init other properties
            this.To = systemClock.UtcNow.Date;
            this.From = this.To.AddDays(-7);
            this.IsTimeTagsEnabled = isTimeTagsEnabled;
            this.IsProjectTagsEnabled = isProjectTagsEnabled;


            this.SetDatesCommand = new Prism.Commands.DelegateCommand<string>(this.SetDates);

        }

        public static List<object> EditConverter(object editValueList)
        {
            if (editValueList is List<object>)
            {
                return editValueList as List<object>;
            }
            else
            {
                return new List<object>();
            }
        }


        private void SetDates(string dateParameter)
        {

            switch (dateParameter)
            {

                case "30D":
                    this.From = this.Clock.UtcNow.AddDays(-30).Date;
                    this.To = this.Clock.UtcNow.Date;
                    break;

                case "90D":
                    this.From = this.Clock.UtcNow.AddDays(-90).Date;
                    this.To = this.Clock.UtcNow.Date;
                    break;

                case "180D":
                    this.From = this.Clock.UtcNow.AddDays(-180).Date;
                    this.To = this.Clock.UtcNow.Date;
                    break;

                case "365D":
                    this.From = this.Clock.UtcNow.AddDays(-365).Date;
                    this.To = this.Clock.UtcNow.Date;
                    break;

                case "LM":
                    this.From = this.Clock.UtcNow.Date.AddMonths(-1).MonthStart();
                    this.To = this.From.MonthEnd();
                    break;

                case "L3M":
                    this.From = this.Clock.UtcNow.Date.AddMonths(-3).MonthStart();
                    this.To = this.Clock.UtcNow.Date.AddMonths(-1).MonthEnd();
                    break;

                case "L6M":
                    this.From = this.Clock.UtcNow.Date.AddMonths(-6).MonthStart();
                    this.To = this.Clock.UtcNow.Date.AddMonths(-1).MonthEnd();
                    break;

                case "YTD":
                    this.From = new DateTime(this.Clock.UtcNow.Year, 1, 1);
                    this.To = this.Clock.UtcNow.Date;
                    break;

                case "MTD":
                    this.From = this.Clock.UtcNow.Date.MonthStart();
                    this.To = this.Clock.UtcNow.Date;
                    break;

            }
        }


        // ---------------------------------
        // item retrieval functions
        public List<User> SelectedUserList() => EditConverter(this.SelectedUsers).Select(s => (User)s).ToList();
        public List<Project> SelectedProjectList() => EditConverter(this.SelectedProjects).Select(s => (Project)s).ToList();
        public List<Tag> SelectedProjectTagList() => EditConverter(this.SelectedProjectTags).Select(s => (Tag)s).ToList();
        public List<Tag> SelectedTaskTagList() => EditConverter(this.SelectedTaskTags).Select(s => (Tag)s).ToList();
        public List<Tag> SelectedTimeTagList() => EditConverter(this.SelectedTimeTags).Select(s => (Tag)s).ToList();


        // ---------------------------------
        // list population helper functions
        public void ClearAndAddUsers(IEnumerable<User> users)
        {
            this.Users.Clear();
            this.Users.AddRange(users);
        }

        public void ClearAndAddProjects(IEnumerable<Project> projects)
        {
            this.Projects.Clear();
            this.Projects.AddRange(projects);
        }

        public void ClearAndAddTags(IEnumerable<Tag> tags)
        {

            // clear out 
            this.ProjectTags.Clear();
            this.TaskTags.Clear();
            this.TimeTags.Clear();

            // add new
            this.ProjectTags.AddRange(tags);
            this.TaskTags.AddRange(tags);
            this.TimeTags.AddRange(tags);

        }

    }
}
