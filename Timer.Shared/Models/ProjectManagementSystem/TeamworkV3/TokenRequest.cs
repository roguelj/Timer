#nullable disable

using Newtonsoft.Json;

namespace Timer.Shared.Models.ProjectManagementSystem.TeamworkV3
{
    public class TokenRequest
    {

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("client_id")]
        public string ClientId { get; set; }

        [JsonProperty("redirect_uri")]
        public string RedirectUri { get; set; }

        [JsonProperty("client_secret")]
        public string ClientSecret { get; set; }

    }
}
