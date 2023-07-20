using Timer.Shared.Models;

namespace Timer.Shared.Services.Interfaces
{
    public interface ITimeLogService
    {

        Task<DateTimeOffset?> GetEndTimeOfLastTimeLogEntryAsync(CancellationToken cancellationToken);
        Task<List<KeyedEntity>?> GetRecentTagsAsync(CancellationToken cancellationToken);
        Task<List<KeyedEntity>?> GetRecentTasksAsync(CancellationToken cancellationToken);
        Task<List<KeyedEntity>?> GetRecentProjectsAsync(CancellationToken cancellationToken);
        Task<bool> LogTime(DateTime startDateTime, DateTime endDateTime, int projectId, int? taskId, List<int> tagIds, bool isBillable, string description, CancellationToken cancellationToken);
        Task<List<KeyedEntity>?> AllProjectsAsync(CancellationToken cancellationToken);

    }

}
