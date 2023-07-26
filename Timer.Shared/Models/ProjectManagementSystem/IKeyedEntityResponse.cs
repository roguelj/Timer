using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timer.Shared.Models.ProjectManagementSystem.TeamworkV3;

namespace Timer.Shared.Models.ProjectManagementSystem
{
    public interface IKeyedEntityResponse<T> where T : IKeyedEntity
    {

        public IEnumerable<T> Items { get; set; }

        public IncludedItems Included { get; set; }

        public Meta Meta { get; set; }
    }
}
