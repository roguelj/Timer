#nullable disable

using Newtonsoft.Json;
using Timer.Shared.Models.ProjectManagementSystem.TeamworkV3.Models;

namespace Timer.Shared.Models.ProjectManagementSystem.TeamworkV3.Responses
{
    public class TimeLogResponse : ResponseBase
    {

        [JsonProperty("timelogs")]
        public IEnumerable<TimeLog> Items { get; set; }

    }
}
