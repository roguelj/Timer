using Timer.Shared.Models;
using Timer.Shared.Models.ProjectManagementSystem;
using Timer.Shared.Models.ProjectManagementSystem.TeamworkV3.Models;

namespace Timer.Shared.Extensions
{
    public static class TeamworkModelExtensions
    {

        public static KeyedEntity ToKeyedEntity(this IKeyedEntity keyedEntity) => new(keyedEntity.Id, keyedEntity.Name);

        public static KeyedEntity ToKeyedEntity(this Project project) => new(project.Id, project.Name);

        public static KeyedEntity ToKeyedEntity(this Tag project) => new(project.Id, project.Name);
    }
}
