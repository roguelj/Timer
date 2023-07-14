namespace Timer.Shared.Models
{
    public class KeyedEntity
    {

        public int Id { get; set; }
        public int ParentId { get; set; }
        public string Name { get; set; }
        public string? Colour { get; set; }

        public KeyedEntity(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        public KeyedEntity(int id, string name, int parentId) : this(id, name) => this.ParentId = parentId;

        public KeyedEntity(int id, string name, int parentId, string colour) : this(id, name, parentId) => this.Colour = colour;

        public KeyedEntity(int id, string name, string colour) : this(id, name) => this.Colour = colour;
    }

}
