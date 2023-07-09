#nullable disable

using Newtonsoft.Json;

namespace Timer.Shared.Models.ProjectManagementSystem.TeamworkV1
{
    public class TasksResponse
    {
        [JsonProperty("STATUS")]
        public string Status { get; set; }

        [JsonProperty("todo-items")]
        public Task[] Tasks { get; set; }

    }
}
