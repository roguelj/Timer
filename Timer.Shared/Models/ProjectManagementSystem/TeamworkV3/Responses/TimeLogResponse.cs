#nullable disable

using Newtonsoft.Json;
using Timer.Shared.Models.ProjectManagementSystem.TeamworkV3.Responses.ResponseMeta;

namespace Timer.Shared.Models.ProjectManagementSystem.TeamworkV3.Responses
{
    public class TimeLogResponse<T> : IKeyedEntityResponse<T> where T : IKeyedEntity
    {

        [JsonProperty("timelogs")]
        public IEnumerable<T> Items { get; set; }

        [JsonProperty("included")]
        public IncludedItems Included { get; set; }

        [JsonProperty("meta")]
        public Meta Meta { get; set; }
    }
}
