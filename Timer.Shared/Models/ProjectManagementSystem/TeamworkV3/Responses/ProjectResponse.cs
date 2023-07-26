#nullable disable

using Newtonsoft.Json;

namespace Timer.Shared.Models.ProjectManagementSystem.TeamworkV3
{
    public class ProjectResponse<T> : IKeyedEntityResponse<T> where T : IKeyedEntity 
    {
        [JsonProperty("projects")]
        public IEnumerable<T> Items { get; set; }

        [JsonProperty("included")]
        public IncludedItems Included { get; set; }

        [JsonProperty("meta")]
        public Meta Meta { get; set; }
    }
}
