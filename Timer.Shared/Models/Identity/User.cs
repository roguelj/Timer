namespace Timer.Shared.Models.Identity
{
    public class User
    {

        [Newtonsoft.Json.JsonProperty("id")]
        public required string Id { get; set; }
        
        [Newtonsoft.Json.JsonProperty("name")]
        public required string Name { get; set; }
        
        [Newtonsoft.Json.JsonProperty("userName")]
        public required string UserName { get; set; }

        [Newtonsoft.Json.JsonProperty("tenantId")]
        public required string TenantId { get; set; }

        [Newtonsoft.Json.JsonProperty("objectId")]
        public required string ObjectId { get; set; }

        [Newtonsoft.Json.JsonProperty("lastAuthActivity")]
        public DateTimeOffset? LastAuthActivity { get; set; }

    }
}
