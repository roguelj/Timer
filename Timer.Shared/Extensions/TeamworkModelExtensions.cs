using Timer.Shared.Models;
using Timer.Shared.Models.ProjectManagementSystem.TeamworkV3;

namespace Timer.Shared.Extensions
{
    public static class TeamworkModelExtensions
    {
        public static KeyedEntity ToKeyedEntity(this Project project)
        {
            return new KeyedEntity(project.Id, project.Name);

        }

        //public static KeyedEntity ToKeyedEntity(this Task task)
        //{
        //    return new KeyedEntity(task.Id, task..Name);

        //}

        public static KeyedEntity ToKeyedEntity(this Tag project)
        {
            return new KeyedEntity(project.Id, project.Name);

        }
    }
}
