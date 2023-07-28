#nullable disable

using Newtonsoft.Json;

namespace Timer.Shared.Models.ProjectManagementSystem.TeamworkV3.Models
{

    public class Task 
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("parentID")]
        public int ParentId { get; set; }

        public Task() { }

        public Task(int id, string name,  int parentId)
        {
            Id = id;
            Name = name;
              ParentId = parentId;
        }
    }

}
