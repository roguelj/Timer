#nullable disable

using Newtonsoft.Json;

namespace Timer.Shared.Models.ProjectManagementSystem.TeamworkV3
{
    public class ProjectResponse
    {
        [JsonProperty("projects")]
        public IEnumerable<Project> Projects { get; set; }

        [JsonProperty("included")]
        public IncludedItems Included { get; set; }

        [JsonProperty("meta")]
        public object Meta { get; set; }
    }
}
