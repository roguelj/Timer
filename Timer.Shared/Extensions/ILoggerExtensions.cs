using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timer.Shared.Extensions
{
    public static partial class Log
    {

        // ------------------------------------------
        // general logs. Event Ids 1100-1199
        [LoggerMessage(EventId = 1100, Level = LogLevel.Critical, Message = "Fatal application exception")]
        public static partial void FatalApplicationException(this ILogger logger, Exception ex);


        [LoggerMessage(EventId = 1101, Level = LogLevel.Error, Message = "Exception during {Method}")]
        public static partial void ExceptionDuringMethod(this ILogger logger, Exception ex, string method);


        [LoggerMessage(EventId = 1102, Level = LogLevel.Information, Message = "Application Startup")]
        public static partial void ApplicationStartup(this ILogger logger);


        [LoggerMessage(EventId = 1103, Level = LogLevel.Information, Message = "Application Shutdown")]
        public static partial void ApplicationShutdown(this ILogger logger);


        [LoggerMessage(EventId = 1104, Level = LogLevel.Debug, Message = "User cancelled")]
        public static partial void UserCancelled(this ILogger logger);




        // ------------------------------------------
        // database related logs. Event Ids 1200-1299
        [LoggerMessage(EventId = 1200, Level = LogLevel.Information, Message = "Created transaction savepoint {SavePoint}")]
        public static partial void CreatedTransactionSavePoint(ILogger logger, string savePoint);


        [LoggerMessage(EventId = 1201, Level = LogLevel.Information, Message = "Rolled back transaction in {Method} for transaction id {Id}")]
        public static partial void RolledBackTransaction(this ILogger logger, string method, Guid id);


        [LoggerMessage(EventId = 1202, Level = LogLevel.Information, Message = "Committed transaction in {Method} for transaction id {Id}")]
        public static partial void CommittedTransaction(this ILogger logger, string method, Guid id);


        [LoggerMessage(EventId = 1203, Level = LogLevel.Error, Message = "Couldn't find entity {EntityName} with id {Id}")]
        public static partial void MissingEntity(this ILogger logger, string entityName, string id);


        [LoggerMessage(EventId = 1204, Level = LogLevel.Error, Message = "DbContext is NULL during {Method}")]
        public static partial void DbContextIsNull(this ILogger logger, string method);


        [LoggerMessage(EventId = 1205, Level = LogLevel.Information, Message = "Saved {ChangeCount} changes to the database")]
        public static partial void SavedChangesToDatabase(this ILogger logger, int changeCount);


        //[LoggerMessage(EventId = 1206, Level = LogLevel.Error, Message = "Exception during database operation in {Method}")]
        //public static partial void DatabaseOperationException(this ILogger logger, DbUpdateException ex, string method);


        // ------------------------------------------
        // prism related logs. Event Ids 1300-1399
        [LoggerMessage(EventId = 1301, Level = LogLevel.Debug, Message = "Initialised {Module} Module")]
        public static partial void InitialisedModule(this ILogger logger, string module);


        [LoggerMessage(EventId = 1302, Level = LogLevel.Debug, Message = "Registered types for {Module} module")]
        public static partial void RegisteredTypesForModule(this ILogger logger, string module);


        [LoggerMessage(EventId = 1303, Level = LogLevel.Debug, Message = "Configuring module catalog. {ModuleCount} modules configured.")]
        public static partial void ConfiguringModuleCatalog(this ILogger logger, int moduleCount);


        [LoggerMessage(EventId = 1304, Level = LogLevel.Information, Message = "Creating Prism shell")]
        public static partial void CreatingPrismShell(this ILogger logger);


        [LoggerMessage(EventId = 1305, Level = LogLevel.Debug, Message = "Configured ViewModel locators")]
        public static partial void ConfiguredViewModelLocators(this ILogger logger);


        [LoggerMessage(EventId = 1306, Level = LogLevel.Debug, Message = "Configured Prism adapters")]
        public static partial void ConfiguredPrismAdapters(this ILogger logger);


        [LoggerMessage(EventId = 1307, Level = LogLevel.Information, Message = "Navigation complete with result {NavigationResult} for context {Context}")]
        public static partial void SuccessfulNavigation(this ILogger logger, bool navigationResult, Uri context);


        [LoggerMessage(EventId = 1308, Level = LogLevel.Error, Message = "Navigation complete with result {NavigationResult} for context {Context}")]
        public static partial void FailedNavigation(this ILogger logger, bool navigationResult, Uri context);


        [LoggerMessage(EventId = 1309, Level = LogLevel.Error, Message = "Navigation complete with result {NavigationResult} for context {Context}. Exception {exception}")]
        public static partial void FailedNavigation(this ILogger logger, bool navigationResult, Uri context, Exception exception);


        // ------------------------------------------
        // operational logs. Event Ids 1400-1499



        // ------------------------------------------
        // authentication related logs. Event Ids 1500-1599
        [LoggerMessage(EventId = 1500, Level = LogLevel.Warning, Message = "MsalUiRequiredException during sign in")]
        public static partial void MsalUiRequired(this ILogger logger, Exception ex);


        [LoggerMessage(EventId = 1501, Level = LogLevel.Information, Message = "Acquired token {AquisitionType} for user {UserId} in tenant {TenantId}")]
        public static partial void TokenAcquired(this ILogger logger, string aquisitionType, string userId, string tenantId);



        public static void LogIdentityClient<T>(this ILogger<T> logger, Microsoft.Identity.Client.LogLevel logLevel, string message)
        {

            var converted =
                logLevel switch
                {
                    Microsoft.Identity.Client.LogLevel.Always => LogLevel.Error,
                    Microsoft.Identity.Client.LogLevel.Error => LogLevel.Error,
                    Microsoft.Identity.Client.LogLevel.Warning => LogLevel.Warning,
                    Microsoft.Identity.Client.LogLevel.Info => LogLevel.Information,
                    Microsoft.Identity.Client.LogLevel.Verbose => LogLevel.Trace,
                    _ => LogLevel.None
                };
            logger.Log(converted, message);
        }

    }

}
