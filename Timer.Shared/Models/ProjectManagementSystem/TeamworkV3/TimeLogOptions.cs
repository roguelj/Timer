using Newtonsoft.Json;

namespace Timer.Shared.Models.ProjectManagementSystem.TeamworkV3
{
    public class TimeLogOptions
    {
        [JsonProperty("fireWebhook")]
        public bool FireWebhook { get; set; }

        [JsonProperty("logActivity")]
        public bool LogActivity { get; set; }

        [JsonProperty("markTaskComplete")]
        public bool MarkTaskComplete { get; set; }

        [JsonProperty("parseInlineTags")]
        public bool ParseInlineTags { get; set; }

        [JsonProperty("useNotifyViaTWIM")]
        public bool UseNotifyViaTWIM { get; set; }

    }

}
