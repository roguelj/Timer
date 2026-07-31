using Newtonsoft.Json;
using System.Text.Json.Serialization;
using Timer.Shared.Models.Identity;

namespace Timer.Shared.Models.Native
{
    public sealed class TimeLogEntryDocument
    {

        [JsonProperty("id")]
        public required string Id { get; set; }


        [JsonProperty("startDateTime")]
        public required DateTimeOffset StartDateTime { get; set; }


        [JsonProperty("endDateTime")]
        public required DateTimeOffset EndDateTime { get; set; }


        [JsonProperty("span")]
        public required TimeSpan Span { get; set; }

        [JsonProperty("durationInSeconds")]
        public required int DurationInSeconds { get; set; }


        [JsonProperty("tagIds")]
        public List<int> TagIds { get; set; } = [];


        [JsonProperty("isBillable")]
        public bool IsBillable { get; set; }


        [JsonProperty("description")]
        public required string Description { get; set; }



        [JsonProperty("userId")]
        public required string UserId { get; set; }

        [JsonProperty("user")]
        public required User User { get; set; }


        [JsonProperty("project")]
        public required Project Project { get; set; }



        [JsonProperty("task")]
        public ProjectTask? Task { get; set; }

    }

}
