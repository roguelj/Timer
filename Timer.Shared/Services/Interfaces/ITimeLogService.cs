using Timer.Shared.Models;

namespace Timer.Shared.Services.Interfaces
{
    public interface ITimeLogService
    {

        Task<DateTimeOffset> GetEndTimeOfLastTimeLogEntryAsync(CancellationToken cancellationToken);
        Task<List<KeyedEntity>> GetRecentTagsAsync();
        Task<List<KeyedEntity>> GetRecentTasksAsync();
        Task<List<KeyedEntity>> GetRecentProjectsAsync();

    }

}
