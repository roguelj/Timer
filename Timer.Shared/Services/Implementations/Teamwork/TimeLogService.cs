using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Serilog;
using Timer.Shared.Models.Options;
using Timer.Shared.Services.Interfaces;

namespace Timer.Shared.Services.Implementations.Teamwork
{
    internal partial class TimeLogService
    {

        // injected services
        private ILogger Logger { get; }
        private IHttpClientFactory HttpClientFactory { get; }
        private IOptions<TeamworkOptions> Options { get; }
        private IMemoryCache MemoryCache { get; }
        private ISystemClock SystemClock { get; }


        // endpoint properties
        private string V1EndpointUrlBase { get => $"{Options.Value.TeamworkEndPointUrlBase}"; }
        private string V3EndpointUrlBase { get => $"{Options.Value.TeamworkEndPointUrlBase}/projects/api/v3"; }


        // generators
        private MemoryCacheEntryOptions MeMemoryCacheEntryOptions { get => new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(8)); }
        private MemoryCacheEntryOptions RecentActivityMemoryCacheEntryOptions { get => new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(30)); }


        // constructor
        public TimeLogService(ILogger logger, IHttpClientFactory httpClientFactory, IOptions<TeamworkOptions> options, IMemoryCache memoryCache, ISystemClock systemClock)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            HttpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            Options = options ?? throw new ArgumentNullException(nameof(options));
            MemoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            SystemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
        }

        // logging
        private async System.Threading.Tasks.Task LogResponseContent(HttpResponseMessage response, CancellationToken cancellationToken)
        {

#if DEBUG
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            Logger.Verbose(responseContent);
#endif
        }

    }

}
