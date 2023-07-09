using Timer.Shared.Models.ProjectManagementSystem.TeamworkV1;
using Timer.Shared.Models.ProjectManagementSystem.TeamworkV3;
using Timer.Shared.ViewModels;

namespace Timer.Shared.Services.Interfaces
{
    public interface ITimeLogService
    {
        //DateTimeOffset GetLastTimePeriodEnd();

        //List<string> GetRecentTags();
        //List<string> GetRecentTasks();
        //List<string> GetRecentProjects();

        Task<Token?> ObtainToken(string temporaryToken, CancellationToken cancellationToken);
        Task<ProjectResponse?> Projects(string token, CancellationToken cancellationToken);
        Task<ProjectResponse?> StarredProjects(string token, CancellationToken cancellationToken);
        Task<TagResponse?> Tags(string token, CancellationToken cancellationToken);
        Task<TasksResponse?> Tasks(string token, ApiQueryParameters apiQueryParameters, bool includeTasksWithNoDueDate, CancellationToken cancellationToken);
        Task<List<TimeLog>> TimeEntries(string token, ApiQueryParameters apiQueryParameters, CancellationToken cancellationToken);
        Task<UserResponse?> Users(string token, CancellationToken cancellationToken);

    }

}
