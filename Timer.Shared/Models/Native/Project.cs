using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace Timer.Shared.Models.Native
{
    public record Project
    {

        [JsonProperty("id")]
        public string? Id { get; set; }


        [JsonProperty("name")]
        public string? Name { get; set; }


        [SetsRequiredMembers]
        public Project(string? id, string? name)
        {
            this.Id = id;
            this.Name = name;
        }

        [SetsRequiredMembers]
        public Project(int id, string? name)
        {
            this.Id = id.ToString();
            this.Name = name;
        }

        public Project()
        {
            this.Id = null;
            this.Name = null;
        }
    }

}
