#nullable disable

using Newtonsoft.Json;

namespace Timer.Shared.Models.ProjectManagementSystem.TeamworkV3
{
    public class TimeLogResponse
    {

        [JsonProperty("timelogs")]
        public IEnumerable<TimeLog> TimeLogs { get; set; }

        [JsonProperty("included")]
        public IncludedItems Included { get; set; }

        [JsonProperty("meta")]
        public Meta Meta { get; set; }
    }
}
