using Prism.Events;
using System.Diagnostics.CodeAnalysis;

namespace Timer.Shared.EventAggregatorEvents
{
    public class SignInEvent : PubSubEvent<AuthenticationPayload> { }

    public class SignOutEvent : PubSubEvent<AuthenticationPayload> { }

    public record AuthenticationPayload
    {

        [SetsRequiredMembers]
        public AuthenticationPayload(DateTimeOffset timestamp, string userName, AuthType authType)
        {
            this.Timestamp = timestamp;
            this.UserName = userName;
            this.AuthType = authType;
        }

        public required DateTimeOffset Timestamp { get; set; }
        public required string UserName { get; set; }

        public AuthType AuthType { get; set; }


    }

    public enum AuthType
    {
        InteractiveSignIn,
        SilentSignIn,
        SignOut
    }
}
