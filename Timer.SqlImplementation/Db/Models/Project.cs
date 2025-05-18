using System.Collections.ObjectModel;

namespace Timer.SqlImplementation.Db.Models
{
    public class Project
    {

        public required int Id { get; set; }

        public required string Name { get; set; }

        public bool IsStarred { get; set; }

        public virtual Collection<ProjectTask> ProjectTasks { get; set; } = [];
    }
}
