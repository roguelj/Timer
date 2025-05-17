#nullable disable

using Newtonsoft.Json;
using Timer.Shared.Models.ProjectManagementSystem.TeamworkV3.Models;
using Timer.Shared.Models.ProjectManagementSystem.TeamworkV3.Responses.ResponseMeta;

namespace Timer.Shared.Models.ProjectManagementSystem.TeamworkV3.Responses
{
    public class UserResponse : ResponseBase
    {

        [JsonProperty("people")]
        public IEnumerable<User> Items { get; set; }



    }
}
