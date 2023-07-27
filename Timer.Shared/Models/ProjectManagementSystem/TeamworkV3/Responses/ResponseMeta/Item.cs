#nullable disable

using Newtonsoft.Json;

namespace Timer.Shared.Models.ProjectManagementSystem.TeamworkV3.Responses.ResponseMeta
{
    public class Item:IKeyedEntity
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("taskListId")]
        public int? TaskListId { get; set; }

    }
}
