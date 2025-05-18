using System.Collections.ObjectModel;

namespace Timer.SqlImplementation.Db.Models
{
    public class User
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public Collection<ProjectTaskUser> ProjectTaskUsers { get; set; } = [];

    }

}
