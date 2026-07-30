using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace Timer.Shared.Models.Native
{
    public record Tag
    {

        [JsonProperty("id")]
        public string? Id { get; set; }


        [JsonProperty("projectId")]
        public string? ProjectId { get; set; }


        [JsonProperty("name")]
        public string? Name { get; set; }


        [JsonProperty("color")]
        public string? Colour { get; set; }


        [SetsRequiredMembers]
        public Tag(string? id, string? name, string? projectId)
        {
            this.Id = id;
            this.Name = name;
            this.ProjectId = projectId;
        }


        [SetsRequiredMembers]
        public Tag(int id, string? name, int projectId)
        {
            this.Id = id.ToString();
            this.Name = name;
            this.ProjectId = projectId.ToString();
        }


        public Tag()
        {
            this.Id = null;
            this.Name = null;
            this.ProjectId = null;
        }   


    }

}
