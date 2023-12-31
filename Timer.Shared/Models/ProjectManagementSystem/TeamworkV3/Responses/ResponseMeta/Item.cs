﻿#nullable disable

using Newtonsoft.Json;

namespace Timer.Shared.Models.ProjectManagementSystem.TeamworkV3.Responses.ResponseMeta
{
    public class Item
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("taskListId")]
        public int? TaskListId { get; set; }

        [JsonIgnore]
        public string Colour { get; set; }

        [JsonProperty("parentId")]
        public int? ParentId { get; set; }
    }
}
