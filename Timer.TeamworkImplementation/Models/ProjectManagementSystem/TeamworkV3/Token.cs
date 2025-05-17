#nullable disable

using Newtonsoft.Json;

namespace Timer.Shared.Models.ProjectManagementSystem.TeamworkV3
{
    public class Token
    {

        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("installation")]
        public Installation Installation { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }
}
