#nullable disable

using Newtonsoft.Json;

namespace Timer.Shared.Models.ProjectManagementSystem.TeamworkV1
{

    public class UserDetailResponse
    {

        [JsonProperty("person")]
        public Person Person { get; set; }

        [JsonProperty("STATUS")]
        public string Status { get; set; }
    }

}
