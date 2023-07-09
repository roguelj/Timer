﻿#nullable disable

using Newtonsoft.Json;

namespace Timer.Shared.Models.ProjectManagementSystem.TeamworkV3
{
    public class Meta
    {
        [JsonProperty("page")]
        public Page Page { get; set; }
    }

    public class Page
    {
        [JsonProperty("pageOffset")]
        public int PageOffset { get; set; }

        [JsonProperty("pageSize")]
        public int PageSize { get; set; }

        [JsonProperty("count")]
        public int PageCount { get; set; }

        [JsonProperty("hasMore")]
        public bool HasMore { get; set; }

    }

}
