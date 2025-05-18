using System.Collections.ObjectModel;

namespace Timer.SqlImplementation.Db.Models
{
    public class ProjectTask
    {

        public required int Id { get; set; }
        public required string Name { get; set; }
        public required int ProjectId { get; set; }
        public required int TaskListId { get; set; }
        public required string TaskListName { get; set; }

        public Collection<User> AssignedUsers { get; set; } = [];

         
    }

}
