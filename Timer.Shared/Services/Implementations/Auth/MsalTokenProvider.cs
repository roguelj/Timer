using Microsoft.Identity.Client;
using Microsoft.Kiota.Abstractions.Authentication;

namespace Timer.Shared.Services.Implementations.Auth
{
    public class MsalTokenProvider : IAccessTokenProvider
    {

        private readonly IPublicClientApplication _pca;
        private readonly string[] _scopes;

        public MsalTokenProvider(IPublicClientApplication pca, string[] scopes)
        {
            _pca = pca;
            _scopes = scopes;
        }

        public AllowedHostsValidator AllowedHostsValidator { get; } = new AllowedHostsValidator(["graph.microsoft.com"]);

        public async Task<string> GetAuthorizationTokenAsync(
            Uri uri,
            Dictionary<string, object>? additionalAuthenticationContext = null,
            CancellationToken cancellationToken = default)
        {
            var account = (await _pca.GetAccountsAsync()).FirstOrDefault();

            var result = await _pca
                .AcquireTokenSilent(_scopes, account)
                .ExecuteAsync(cancellationToken);

            return result.AccessToken;
        }
    }
}
