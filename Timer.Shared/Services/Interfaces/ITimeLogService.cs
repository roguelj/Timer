﻿using Timer.Shared.Models;
using Timer.Shared.Models.ProjectManagementSystem.TeamworkV3.Models;

namespace Timer.Shared.Services.Interfaces
{
    public interface ITimeLogService
    {


        /// <summary>
        /// Get all Projects
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>A List of Projects as keyed entities</returns>
        Task<List<Project>?> Projects(CancellationToken cancellationToken);

        /// <summary>
        /// Get all Tasks
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>A List of Tasks as keyed entities</returns>
        Task<List<Models.ProjectManagementSystem.TeamworkV3.Models.Task>?> Tasks(CancellationToken cancellationToken);

        /// <summary>
        /// Get all Tags
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>A List of Tags as keyed entities</returns>
        Task<List<Tag>?> Tags(CancellationToken cancellationToken);


        /// <summary>
        /// Get all Projects that match the search criteria.
        /// </summary>
        /// <param name="searchCriteria">The text to search for</param>
        /// <param name="cancellationToken"></param>
        /// <returns>A List of Projects as keyed entities</returns>
        Task<List<Project>?> Projects(string searchCriteria, CancellationToken cancellationToken);

        /// <summary>
        /// Get all Tasks that match the search criteria.
        /// </summary>
        /// <param name="searchCriteria">The text to search for</param>
        /// <param name="cancellationToken"></param>
        /// <returns>A List of Tasks as keyed entities</returns>
        Task<List<Shared.Models.ProjectManagementSystem.TeamworkV3.Models.Task>?> Tasks(string searchCriteria, CancellationToken cancellationToken);

        /// <summary>
        /// Get all Tags that match the search criteria.
        /// </summary>
        /// <param name="searchCriteria">The text to search for</param>
        /// <param name="cancellationToken"></param>
        /// <returns>A List of Tags as keyed entities</returns>
        Task<List<Tag>?> Tags(string searchCriteria, CancellationToken cancellationToken);

        /// <summary>
        /// Get Projects that have time logged against them recently.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>A List of Projects as keyed entities</returns>
        Task<List<Project>?> RecentProjects(CancellationToken cancellationToken);

        /// <summary>
        /// Get Tasks that have time logged against them recently.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>A List of Tasks as keyed entities</returns>
        Task<List<Shared.Models.ProjectManagementSystem.TeamworkV3.Models.Task>?> RecentTasks(CancellationToken cancellationToken);

        /// <summary>
        /// Get Tags that have time logged against them recently.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>A List of Tags as keyed entities</returns>
        Task<List<Tag>?> RecentTags(CancellationToken cancellationToken);


        /// <summary>
        /// Get the DateTime of the end of the last time entry
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<DateTimeOffset?> GetEndTimeOfLastTimeLogEntryAsync(CancellationToken cancellationToken);


        /// <summary>
        /// Creates a time log entry.
        /// </summary>
        /// <param name="startDateTime"></param>
        /// <param name="endDateTime"></param>
        /// <param name="projectId"></param>
        /// <param name="taskId"></param>
        /// <param name="tagIds"></param>
        /// <param name="isBillable"></param>
        /// <param name="description"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> LogTime(DateTime startDateTime, DateTime endDateTime, int projectId, int? taskId, List<int> tagIds, bool isBillable, string description, CancellationToken cancellationToken);

    }

}
