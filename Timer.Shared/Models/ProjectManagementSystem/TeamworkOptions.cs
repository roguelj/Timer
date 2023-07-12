#nullable disable

namespace Timer.Shared.Models.ProjectManagementSystem
{
    public class TeamworkOptions
    {
        public string TokenRequestUrl { get; set; }
        public string TeamworkEndPointUrlBase { get; set; }
        public string LoginUrl { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectUri { get; set; }
        public string ApiKey { get; set; }
        public string AuthType { get; set; }
        public int? DaysToConsiderRecent { get; set; }
    }

}
