namespace Timer.Shared.Models
{
    public class KeyedEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Dictionary<string, object> Data { get; }

        public KeyedEntity(int id, string name)
        {
            this.Id = id;
            this.Name = name;
            this.Data = new Dictionary<string, object>();
        }

        public KeyedEntity(int id, string name, Dictionary<string, object> data) : this(id, name) 
        {
            this.Data = data;
        }

    }

}
