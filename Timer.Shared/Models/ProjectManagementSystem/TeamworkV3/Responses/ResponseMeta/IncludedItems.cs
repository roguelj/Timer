#nullable disable

using Newtonsoft.Json;
using Timer.Shared.Models.ProjectManagementSystem.TeamworkV3.Models;

namespace Timer.Shared.Models.ProjectManagementSystem.TeamworkV3.Responses.ResponseMeta
{
    public class IncludedItems
    {
        [JsonProperty("tasks")]
        public Dictionary<int, Item> Tasks { get; set; }

        [JsonProperty("tasklists")]
        public Dictionary<int, Item> TaskLists { get; set; }

        [JsonProperty("projects")]
        public Dictionary<int, Item> Projects { get; set; }

        [JsonProperty("tags")]
        public Dictionary<int, Tag> Tags { get; set; }

        [JsonProperty("users")]
        public Dictionary<int, User> Users { get; set; }


    }
}
