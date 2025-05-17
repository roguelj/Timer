using Newtonsoft.Json;

namespace Timer.Shared.Models.ProjectManagementSystem.TeamworkV3.Models
{
    public class TimeLogInput
    {

        [JsonProperty("description")]
        public string? Description { get; set; }

        [JsonProperty("hasStartTime")]
        public bool? HasStartTime { get; set; }

        [JsonProperty("taskId", NullValueHandling = NullValueHandling.Ignore)]
        public int? TaskId { get; set; }

        [JsonProperty("projectId", NullValueHandling = NullValueHandling.Ignore)]
        public int? ProjectId { get; set; }

        [JsonProperty("tagIds", NullValueHandling = NullValueHandling.Ignore)]
        public List<int> TagIds { get; set; }

        [JsonProperty("date", NullValueHandling = NullValueHandling.Ignore)]
        public string? Date { get; set; }

        [JsonProperty("time", NullValueHandling = NullValueHandling.Ignore)]
        public string? Time { get; set; }

        [JsonProperty("minutes", NullValueHandling = NullValueHandling.Ignore)]
        public int? Minutes { get; set; }

        [JsonProperty("hours", NullValueHandling = NullValueHandling.Ignore)]
        public int? Hours { get; set; }

        [JsonProperty("isBillable", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsBillable { get; set; }

    }

}
