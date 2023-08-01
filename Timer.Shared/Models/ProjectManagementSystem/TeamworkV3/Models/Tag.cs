#nullable disable

using Newtonsoft.Json;

namespace Timer.Shared.Models.ProjectManagementSystem.TeamworkV3.Models
{
    public class Tag 
    {

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("projectId")]
        public int ProjectId { get; set; }

        [JsonProperty("project")]
        public SubType Project { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("color")]
        public string Colour { get; set; }

        public Tag() { }

        public Tag(int id, string name, string colour)
        {
            this.Id = id;
            this.Name = name;
            this.Colour = colour;
        }
    }

}
