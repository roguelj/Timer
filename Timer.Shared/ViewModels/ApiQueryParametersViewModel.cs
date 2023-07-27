using Prism.Mvvm;
using Timer.Shared.Models.ProjectManagementSystem.TeamworkV3.Models;

namespace Timer.Shared.ViewModels
{
    public class ApiQueryParameters : BindableBase
    {


        // parameter properties
        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public List<User> SelectedUsers { get; } = new List<User>();

        public List<Project> SelectedProjects { get; } = new List<Project>();

        public List<Tag> SelectedProjectTags { get; } = new List<Tag>();

        public List<Tag> SelectedTaskTags { get; } = new List<Tag>();

        public List<Tag> SelectedTimeTags { get; } = new List<Tag>();


        public ApiQueryParameters(DateTime from, DateTime to)
        {
            this.From = from;
            this.To = to;
        }

        public ApiQueryParameters(DateTime from, DateTime to, User user) : this(from, to) => this.SelectedUsers.Add(user);

    }

}
