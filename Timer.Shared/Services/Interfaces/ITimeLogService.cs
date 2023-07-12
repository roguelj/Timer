using Timer.Shared.Models;

namespace Timer.Shared.Services.Interfaces
{
    public interface ITimeLogService
    {

        Task<DateTimeOffset> GetEndTimeOfLastTimeLogEntryAsync(CancellationToken cancellationToken);
        Task<List<KeyedEntity>?> GetRecentTagsAsync(CancellationToken cancellationToken);
        Task<List<KeyedEntity>?> GetRecentTasksAsync(CancellationToken cancellationToken);
        Task<List<KeyedEntity>?> GetRecentProjectsAsync(CancellationToken cancellationToken);

    }

}
