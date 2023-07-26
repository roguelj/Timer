namespace Timer.Shared.Models
{
    public class KeyedEntities
    {
        public KeyedEntities(List<KeyedEntity> projects, List<KeyedEntity> tasks, List<KeyedEntity> tags)
        {
            Projects = projects;
            Tasks = tasks;
            Tags = tags;
        }

        public List<KeyedEntity> Projects { get; }
        public List<KeyedEntity> Tasks { get; }
        public List<KeyedEntity> Tags { get; }
    }
}
