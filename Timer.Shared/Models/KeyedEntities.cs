using Timer.Shared.Models.ProjectManagementSystem.TeamworkV3.Models;

namespace Timer.Shared.Models
{
    public class KeyedEntities
    {
        public KeyedEntities(List<KeyedEntity> projects, List<KeyedEntity> tasks, List<Tag> tags)
        {
            Projects = projects;
            Tasks = tasks;
            Tags = tags;
        }

        public List<KeyedEntity> Projects { get; }
        public List<KeyedEntity> Tasks { get; }
        public List<Tag> Tags { get; }
    }
}
