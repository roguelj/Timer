using Timer.Shared.Models;
using Timer.Shared.Models.ProjectManagementSystem.TeamworkV1;
using Timer.Shared.Services.Interfaces;
using LogRes = Timer.Shared.Resources.LogMessages;

namespace Timer.Shared.Services.Implementations
{

    internal partial class TeamworkTimeLogService : ITimeLogService
    {

        async Task<DateTimeOffset?> ITimeLogService.GetEndTimeOfLastTimeLogEntryAsync(CancellationToken cancellationToken)
        {

            if (await this.Me(cancellationToken) is Person currentUser)
            {

                // get the last entry
                var entry = (await this.MyLastTimeEntry(currentUser.Id, cancellationToken));

                // get the datetime of the last entry, or the current datetime if there is none
                var lastTimeEntry = (entry?.TimeLogged ?? this.SystemClock.UtcNow);

                // get the duration of the last entry
                var durationMinutes = entry?.Minutes ?? 0;

                // adjust entry datetime to local
                lastTimeEntry = TimeZoneInfo.ConvertTimeFromUtc(lastTimeEntry.DateTime, TimeZoneInfo.Local);

                // return the end of the last time entry to the caller
                return lastTimeEntry.AddMinutes(durationMinutes);

            }
            else
            {
                this.Logger.Error(LogRes.UnknownUser);
                return null;

            }

        }

        async Task<List<KeyedEntity>?> ITimeLogService.GetRecentProjectsAsync(CancellationToken cancellationToken)
        {

            if (await this.Me(cancellationToken) is Person currentUser)
            {
                return (await this.MyRecentProjects(currentUser.Id, cancellationToken));

            }
            else
            {
                this.Logger.Error(LogRes.UnknownUser);
                return null;

            }
           
        }

        async Task<List<KeyedEntity>?> ITimeLogService.GetRecentTagsAsync(CancellationToken cancellationToken)
        {

            if (await this.Me(cancellationToken) is Person currentUser)
            {
                return (await this.MyRecentTags(currentUser.Id, cancellationToken));

            }
            else
            {
                this.Logger.Error(LogRes.UnknownUser);
                return null;

            }

        }

        async Task<List<KeyedEntity>?> ITimeLogService.GetRecentTasksAsync(CancellationToken cancellationToken)
        {

            if (await this.Me(cancellationToken) is Person currentUser)
            {
                return (await this.MyRecentTasks(currentUser.Id, cancellationToken));

            }
            else
            {
                this.Logger.Error(LogRes.UnknownUser);
                return null;

            }

        }

        async Task<bool> ITimeLogService.LogTime(DateTime startDateTime, DateTime endDateTime, int projectId, int? taskId, List<int> tagIds, bool isBillable, string description, CancellationToken cancellationToken)
        {
            return await this.CreateTimeEntry(startDateTime, endDateTime, projectId, taskId, tagIds,isBillable, description,cancellationToken);
        }

    }

}
