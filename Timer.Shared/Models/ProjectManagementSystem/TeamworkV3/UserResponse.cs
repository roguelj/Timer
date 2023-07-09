#nullable disable

using Newtonsoft.Json;

namespace Timer.Shared.Models.ProjectManagementSystem.TeamworkV3
{
    public class UserResponse
    {

        [JsonProperty("people")]
        public IEnumerable<User> Users { get; set; }

        [JsonProperty("included")]
        public object Included { get; set; }


        [JsonProperty("meta")]
        public object Meta { get; set; }

    }
}
