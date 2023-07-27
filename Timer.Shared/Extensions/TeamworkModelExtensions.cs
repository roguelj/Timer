using Timer.Shared.Models;
using Timer.Shared.Models.ProjectManagementSystem;
using Timer.Shared.Models.ProjectManagementSystem.TeamworkV3.Models;

namespace Timer.Shared.Extensions
{
    public static class TeamworkModelExtensions
    {

        public static KeyedEntity ToKeyedEntity(this IKeyedEntity keyedEntity) => new(keyedEntity.Id, keyedEntity.Name, keyedEntity.Colour);

        public static Tag ToTag(this IKeyedEntity keyedEntity) => new Tag { Id = keyedEntity.Id, Name = keyedEntity.Name, Colour = keyedEntity.Colour };
    }
}
