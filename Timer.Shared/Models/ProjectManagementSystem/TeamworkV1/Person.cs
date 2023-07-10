#nullable disable

using Newtonsoft.Json;

namespace Timer.Shared.Models.ProjectManagementSystem.TeamworkV1
{
    public class Person
    {

        [JsonProperty("administrator")]
        public bool Administrator { get; set; }

        [JsonProperty("pid")]
        public string Pid { get; set; }

        [JsonProperty("siteowner")]
        public bool SiteOwner { get; set; }

        [JsonProperty("twitter")]
        public string Twitter { get; set; }

        [JsonProperty("phonenumberhome")]
        public string PhoneNumberHome { get; set; }

        [JsonProperty("lastname")]
        public string LastName { get; set; }

        [JsonProperty("emailaddress")]
        public string EmailAddress { get; set; }

        [JsonProperty("profile")]
        public string Profile { get; set; }

        [JsonProperty("userUUID")]
        public string UserUUID { get; set; }

        [JsonProperty("username")]
        public string UserName { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("companyname")]
        public string CompanyName { get; set; }

        [JsonProperty("lastchangedon")]
        public DateTime LastChangedOn { get; set; }

        [JsonProperty("phonenumberoffice")]
        public string PhoneNumberOffice { get; set; }

        [JsonProperty("deleted")]
        public bool Deleted { get; set; }

        [JsonProperty("privateNotes")]
        public string PrivateNotes { get; set; }

        [JsonProperty("phonenumbermobile")]
        public string PhoneNumberMobile { get; set; }

        [JsonProperty("firstname")]
        public string FirstName { get; set; }

        [JsonProperty("usertype")]
        public string UserType { get; set; }

        [JsonProperty("permissions")]
        public Permissions Permissions { get; set; }

        [JsonProperty("imservice")]
        public string ImService { get; set; }

        [JsonProperty("imhandle")]
        public string ImHandle { get; set; }

        [JsonProperty("createdat")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("phonenumberofficeext")]
        public string PhoneNumberOfficeExt { get; set; }

        [JsonProperty("companyid")]
        public string CompanyId { get; set; }

        [JsonProperty("hasaccesstonewprojects")]
        public bool HasAccessToNewProjects { get; set; }

        [JsonProperty("phonenumberfax")]
        public string PhoneNumberFax { get; set; }

        [JsonProperty("avatarurl")]
        public string AvatarUrl { get; set; }

        [JsonProperty("inownercompany")]
        public string InOwnerCompany { get; set; }

        [JsonProperty("lastlogin")]
        public DateTime LastLogin { get; set; }

        [JsonProperty("emailalt1")]
        public string EmailAlt1 { get; set; }

        [JsonProperty("emailalt2")]
        public string EmailAlt2 { get; set; }

        [JsonProperty("emailalt3")]
        public string EmailAlt3 { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

    }

    public class Permissions
    {

        [JsonProperty("canmanagepeople")]
        public bool CanManagePeople { get; set; }

        [JsonProperty("canaddprojects")]
        public bool CanAddProjects { get; set; }
    }

}
