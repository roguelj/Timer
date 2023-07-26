using Timer.Shared.Models;
using Timer.Shared.Models.ProjectManagementSystem.TeamworkV1;
using Timer.Shared.Services.Interfaces;
using LogRes = Timer.Shared.Resources.LogMessages;

namespace Timer.Shared.Services.Implementations
{

    internal partial class TeamworkTimeLogService : ITimeLogService
    {

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


    }

}
