using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timer.Shared.Models;
using Timer.Shared.Services.Interfaces;

namespace Timer.Shared.Services.Implementations
{
    internal partial class TeamworkTimeLogService : ITimeLogService
    {
        DateTimeOffset ITimeLogService.GetEndTimeOfLastTimeLogEntry()
        {
            throw new NotImplementedException();
        }

        List<KeyedEntity> ITimeLogService.GetRecentProjects()
        {
            throw new NotImplementedException();
        }

        List<KeyedEntity> ITimeLogService.GetRecentTags()
        {
            throw new NotImplementedException();
        }

        List<KeyedEntity> ITimeLogService.GetRecentTasks()
        {
            throw new NotImplementedException();
        }
    }

}
