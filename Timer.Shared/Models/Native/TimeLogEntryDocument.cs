using Newtonsoft.Json;
using System.Text.Json.Serialization;
using Timer.Shared.Models.Identity;

namespace Timer.Shared.Models.Native
{
    public sealed class TimeLogEntryDocument
    {

        [JsonProperty("id")]
        public required string Id { get; set; }


        [JsonPropertyName("startDateTime")]
        public required DateTimeOffset StartDateTime { get; set; }


        [JsonPropertyName("endDateTime")]
        public required DateTimeOffset EndDateTime { get; set; }


        [JsonPropertyName("tagIds")]
        public List<int> TagIds { get; set; } = [];


        [JsonPropertyName("isBillable")]
        public bool IsBillable { get; set; }


        [JsonPropertyName("description")]
        public required string Description { get; set; }



        [JsonPropertyName("userId")]
        public required string UserId { get; set; }

        [JsonPropertyName("user")]
        public required User User { get; set; }


        [JsonPropertyName("projectId")]
        public required string ProjectId { get; set; }

        [JsonPropertyName("project")]
        public required Project Project { get; set; }



        [JsonPropertyName("taskId")]
        public required string TaskId { get; set; }

        [JsonPropertyName("task")]
        public required ProjectTask Task { get; set; }

    }

}
