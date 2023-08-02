#nullable disable

using Newtonsoft.Json;

namespace Timer.Shared.Models.ProjectManagementSystem.TeamworkV3.Models
{
    public class Project
    {

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        public Project() { }

        public Project(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }
    }
}
