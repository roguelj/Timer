using Azure.Identity;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using Microsoft.Kiota.Abstractions.Authentication;
using Prism.Events;
using System.Security.Cryptography;
using Timer.Shared.Extensions;
using Timer.Shared.Models.Identity;
using Timer.Shared.Models.Options;

namespace Timer.Shared.Services.Implementations.Auth
{

    public class AuthService
    {

        // https://learn.microsoft.com/en-us/entra/identity-platform/tutorial-desktop-wpf-dotnet-sign-in-build-app

        // ------------------------
        // injected services
        private ILogger<AuthService> Logger { get; }

        private ILogger<PublicClientApplication> MsalLogger { get; }

        private IOptions<EntraOptions> Options { get; }

        private IPublicClientApplication PublicClientApp { get; }

        private IEventAggregator EventAggregator { get; }

        private TimeProvider TimeProvider { get; }

        private IOptions<TokenCacheOptions> TokenCacheOptions { get; }

        private CosmosClient CosmosClient { get; }


        // ------------------------
        // properties
        private static readonly System.Threading.Lock FileLock = new();

        private string CacheFilePath { get; }

        private List<string> Scopes { get; } = ["User.Read", "Tasks.Read"];


        private const string DatabaseId = "TimerDb";
        private const string ContainerId = "Users";

        public Timer.Shared.Models.Identity.User? LoggedInUser { get; private set; }


        private GraphServiceClient? graphClient;
        public GraphServiceClient GraphClient
        {
            get
            {
                if(graphClient == null)
                {

                    var tokenProvider = new MsalTokenProvider(this.PublicClientApp, this.Scopes.ToArray());
            
                    var authProvider = new BaseBearerTokenAuthenticationProvider(tokenProvider);

                    graphClient = new GraphServiceClient(authProvider);
                }
                return graphClient;
            }
        }




        // ------------------------
        // constructor
        public AuthService(
            ILogger<AuthService> logger,
                            ILogger<PublicClientApplication> msalLogger,
                            IOptions<EntraOptions> options,
                            IOptions<TokenCacheOptions> tokenCacheOptions,
                            IEventAggregator eventAggregator,
                            TimeProvider timeProvider,
                            CosmosClient cosmosClient)
        {

            // set up injected services
            this.Options = options ?? throw new ArgumentNullException(nameof(options));
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.CosmosClient = cosmosClient ?? throw new ArgumentNullException(nameof(cosmosClient));
            this.MsalLogger = msalLogger ?? throw new ArgumentNullException(nameof(msalLogger));
            this.EventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
            this.TimeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
            this.TokenCacheOptions = tokenCacheOptions ?? throw new ArgumentNullException(nameof(tokenCacheOptions));


            var logCallback = new LogCallback((logLevel, message, containsPII) => this.MsalLogger.LogIdentityClient(logLevel, message));


            // configure the PublicClientApp
            this.PublicClientApp = PublicClientApplicationBuilder.Create(options.Value.ClientId)
                                   .WithAuthority(options.Value.Authority)
                                   .WithDefaultRedirectUri()
                                   .WithLogging(logCallback, enablePiiLogging: false)
                                   .Build();


            this.PublicClientApp.UserTokenCache.SetBeforeAccess(this.BeforeAccessNotification);
            this.PublicClientApp.UserTokenCache.SetAfterAccess(this.AfterAccessNotification);

            try
            {
                var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                this.CacheFilePath = Path.Combine(folderPath, this.TokenCacheOptions.Value.FileName);
            }
            catch (InvalidOperationException ex)
            {
                this.Logger.LogError(ex, "AuthService ctor");
                throw;
            }

        }


        // ------------------------
        // private methods
        private void BeforeAccessNotification(TokenCacheNotificationArgs args)
        {

            lock (FileLock)
            {

                if (File.Exists(this.CacheFilePath) == false)
                {
                    args.TokenCache.DeserializeMsalV3(null);
                    return;
                }

                var entropy = this.TokenCacheOptions.Value.Entropy.Concat(Constants.Application.ENTROPY).ToArray();

                var unprotectedBytes = ProtectedData.Unprotect(File.ReadAllBytes(this.CacheFilePath),
                                                 entropy,
                                                 DataProtectionScope.CurrentUser);

                args.TokenCache.DeserializeMsalV3(unprotectedBytes);

            }

        }

        private void AfterAccessNotification(TokenCacheNotificationArgs args)
        {

            if (args.HasStateChanged)
            {
                lock (FileLock)
                {

                    var entropy = this.TokenCacheOptions.Value.Entropy.Concat(Constants.Application.ENTROPY).ToArray();

                    var protectedBytes = ProtectedData.Protect(args.TokenCache.SerializeMsalV3(), entropy, DataProtectionScope.CurrentUser);
                    File.WriteAllBytes(this.CacheFilePath, protectedBytes);

                }

            }

        }

        private async Task UpdateDb(AuthenticationResult authresult, CancellationToken cancellationToken)
        {
            var container = await this.GetContainerAsync(cancellationToken);

            var user = new Models.Identity.User
            {
                Id = authresult.Account.HomeAccountId.ObjectId,
                Name = authresult.Account.Username,
                UserName = authresult.Account.Username,
                ObjectId = authresult.Account.HomeAccountId.ObjectId,
                TenantId = authresult.TenantId,
                LastAuthActivity = this.TimeProvider.GetUtcNow()
            };

            try
            {
                var response = await container.UpsertItemAsync(user, new PartitionKey(user.TenantId), cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "Error creating user in Cosmos DB");
            }


            this.LoggedInUser = user;
        
        }

        private async Task<Container> GetContainerAsync(CancellationToken cancellationToken)
        {
            var database = (await this.CosmosClient.CreateDatabaseIfNotExistsAsync(DatabaseId, cancellationToken: cancellationToken)).Database;
            var container = (await database.CreateContainerIfNotExistsAsync(ContainerId, "/tenantId", cancellationToken: cancellationToken)).Container;
            return container;
        }


        // ------------------------
        // public implementations

        /// <summary>
        /// Sign In
        /// </summary>
        /// <param name="windowHandle">Use new WindowInteropHelper(window).Handle to get this</param>
        /// <returns></returns>
        public async Task SignIn(nint windowHandle)
        {

            AuthenticationResult? authResult = null;

            // get accounts
            var accounts = await this.PublicClientApp.GetAccountsAsync();

            try
            {
                authResult = await this.PublicClientApp
                                        .AcquireTokenSilent(this.Scopes, accounts.FirstOrDefault())
                                        .ExecuteAsync();

                this.Logger.TokenAcquired("silently", authResult.Account.HomeAccountId.ObjectId, authResult.TenantId);

                this.EventAggregator.PublishNotification(this.TimeProvider, $"Signed in as {authResult.Account.Username}", EventAggregatorEvents.NotificationLevel.Information);
                this.EventAggregator.PublishSilentSignIn(this.TimeProvider, authResult.Account.Username);

                await this.UpdateDb(authResult, CancellationToken.None);

            }
            catch (MsalUiRequiredException ex)
            {

                this.Logger.MsalUiRequired(ex);

                try
                {

                    // attempt interactive login
                    authResult = await this.PublicClientApp.AcquireTokenInteractive(this.Scopes)
                                        .WithAccount(accounts.FirstOrDefault())
                                        .WithParentActivityOrWindow(windowHandle)
                                        .WithPrompt(Prompt.SelectAccount)
                                        .ExecuteAsync();

                    this.Logger.TokenAcquired("interactively", authResult.Account.HomeAccountId.ObjectId, authResult.TenantId);

                    this.EventAggregator.PublishNotification(this.TimeProvider, $"Signed in as {authResult.Account.Username}", EventAggregatorEvents.NotificationLevel.Information);
                    await this.UpdateDb(authResult, CancellationToken.None);

                    this.EventAggregator.PublishInteractiveSignIn(this.TimeProvider, authResult.Account.Username);
                }
                catch (MsalException msalex)
                {
                    this.Logger.LogError(msalex, nameof(SignIn));
                }
                catch (Exception exception)
                {
                    this.Logger.ExceptionDuringMethod(exception, nameof(SignIn));
                    return;
                }

            }

        }


        /// <summary>
        /// Sign Out
        /// </summary>
        /// <returns></returns>
        public async Task<bool?> SignOut()
        {

            var accounts = await this.PublicClientApp.GetAccountsAsync();
            if (accounts.Any())
            {
                try
                {
                    this.EventAggregator.PublishSignOut(this.TimeProvider, this.LoggedInUser?.UserName ?? string.Empty);
                    await this.PublicClientApp.RemoveAsync(accounts.FirstOrDefault());
                    this.EventAggregator.PublishNotification(this.TimeProvider, "Signed out", EventAggregatorEvents.NotificationLevel.Information);

                    return true;
                }
                catch (MsalException ex)
                {
                    this.Logger.ExceptionDuringMethod(ex, nameof(SignOut));
                    return false;
                }

            }

            return null;

        }

    }

}
