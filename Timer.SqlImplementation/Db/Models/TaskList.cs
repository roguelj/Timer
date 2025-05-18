using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timer.SqlImplementation.Db.Models
{
    public class TaskList
    {
        public required int Id { get; set; }
        public required int ProjectId { get; set; }
        public required string Name { get; set; }
        public virtual Project? Project { get; set; } = null!;
        public virtual ICollection<ProjectTask> ProjectTasks { get; set; } = new HashSet<ProjectTask>();
    }
}
