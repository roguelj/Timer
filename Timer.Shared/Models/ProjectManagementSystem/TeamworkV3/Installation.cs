#nullable disable

using Newtonsoft.Json;
using Timer.Shared.Models.ProjectManagementSystem.TeamworkV3.Models;

namespace Timer.Shared.Models.ProjectManagementSystem.TeamworkV3
{
    public class Installation
    {

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("apiEndPoint")]
        public string ApiEndPoint { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("company")]
        public Company Company { get; set; }

        [JsonProperty("logo")]
        public string Logo { get; set; }
    }
}
