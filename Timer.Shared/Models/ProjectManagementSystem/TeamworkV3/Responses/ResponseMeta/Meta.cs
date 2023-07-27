#nullable disable

using Newtonsoft.Json;

namespace Timer.Shared.Models.ProjectManagementSystem.TeamworkV3.Responses.ResponseMeta
{
    public class Meta
    {
        [JsonProperty("page")]
        public Page Page { get; set; }
    }

}
