using Timer.Shared.Models;
using Timer.Shared.Services.Interfaces;

namespace Timer.Shared.Services.Implementations
{
    internal partial class TeamworkTimeLogService : ITimeLogService
    {
        async Task<DateTimeOffset?> ITimeLogService.GetEndTimeOfLastTimeLogEntryAsync(CancellationToken cancellationToken)
        {
            var me = await this.Me(cancellationToken); 
            var lastTimeEntry = (await this.MyLastTimeEntry(me.Id, cancellationToken));

            return (lastTimeEntry?.TimeLogged ?? this.SystemClock.UtcNow).AddMinutes(lastTimeEntry?.Minutes ?? 0);

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

        async Task<bool> ITimeLogService.LogTime(DateTime startDateTime, DateTime endDateTime, int projectId, int? taskId, List<int> tagIds, CancellationToken cancellationToken)
        {
            return await this.CreateTimeEntry(startDateTime, endDateTime, projectId, taskId, tagIds, cancellationToken);
        }

    }

}
