using System.Threading;
using Timer.Shared.Models;
using Timer.Shared.Services.Interfaces;

namespace Timer.Shared.Services.Implementations
{
    internal partial class TeamworkTimeLogService : ITimeLogService
    {
        async Task<DateTimeOffset> ITimeLogService.GetEndTimeOfLastTimeLogEntryAsync(CancellationToken cancellationToken)
        {
            var me = await this.Me(cancellationToken); 
            return (await this.MyLastTimeEntry(me.Id, cancellationToken)).TimeLogged;
        }

        async Task<List<KeyedEntity>?> ITimeLogService.GetRecentProjectsAsync(CancellationToken cancellationToken)
        {
            var me = await this.Me(cancellationToken);
            return (await this.MyRecentProjects(me.Id, cancellationToken));
        }

        async Task<List<KeyedEntity>?> ITimeLogService.GetRecentTagsAsync(CancellationToken cancellationToken)
        {
            var me = await this.Me(cancellationToken);
            return (await this.MyRecentTags(me.Id, cancellationToken));
        }

        async Task<List<KeyedEntity>?> ITimeLogService.GetRecentTasksAsync(CancellationToken cancellationToken)
        {
            var me = await this.Me(cancellationToken);
            return (await this.MyRecentTasks(me.Id, cancellationToken));
        }

    }

}
