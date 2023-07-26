using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timer.Shared.Models.ProjectManagementSystem
{
    public interface IKeyedEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
