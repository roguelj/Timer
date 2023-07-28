#nullable disable

using Newtonsoft.Json;
using Timer.Shared.Models.ProjectManagementSystem.TeamworkV3.Responses;

namespace Timer.Shared.Models.ProjectManagementSystem.TeamworkV3
{
    public class TaskResponse : ResponseBase
    {
        [JsonProperty("tasks")]
        public IEnumerable<Models.Task> Items { get; set; }


    }
}
