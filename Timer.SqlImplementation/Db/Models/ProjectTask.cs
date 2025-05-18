using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timer.SqlImplementation.Db.Models
{
    public class ProjectTask
    {

        public required int Id { get; set; }
        public required string Name { get; set; }
        public required int ProjectId { get; set; }
        public required int TaskListId { get; set; }
        public required string TaskListName { get; set; }

        public Collection<ProjectTaskUser> ProjectTaskUsers { get; set; } = [];

    }

}
