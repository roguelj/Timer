﻿using Timer.Shared.Models;

namespace Timer.Shared.Services.Interfaces
{
    public interface ITimeLogService
    {

        Task<DateTimeOffset> GetEndTimeOfLastTimeLogEntryAsync();
        Task<List<KeyedEntity>> GetRecentTagsAsync();
        Task<List<KeyedEntity>> GetRecentTasksAsync();
        Task<List<KeyedEntity>> GetRecentProjectsAsync();

    }

}
