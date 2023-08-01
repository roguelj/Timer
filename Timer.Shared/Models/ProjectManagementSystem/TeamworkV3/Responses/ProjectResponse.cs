#nullable disable

using Newtonsoft.Json;
using Timer.Shared.Models.ProjectManagementSystem.TeamworkV3.Models;
using Timer.Shared.Models.ProjectManagementSystem.TeamworkV3.Responses;

namespace Timer.Shared.Models.ProjectManagementSystem.TeamworkV3
{
    public class ProjectResponse : ResponseBase
    {
        [JsonProperty("projects")]
        public IEnumerable<Project> Items { get; set; }


    }
}
