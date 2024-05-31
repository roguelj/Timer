namespace Timer.Shared.Constants
{
    public static class LogMessage
    {
            
        public const string HTTP_STATUS_CODE = "Status code {code} response";

        public const string HTTP_STATUS_CODE_FOR = "Status code {code} response for {url}";

        public const string RESPONSE_READ_FAILURE = "Failed to read Http response";

        public const string HTTP_RESPONSE_CONTENT = "Response: {response}";

        public const string UNKNOWN_USER = "Unknown user";

        public const string LOG_TIME_FAILURE = "Log time attempt failure";

        public const string ENTITY_COUNT = "Found {count} {entity}(s)";

        public const string RECENT_FAILURE = "Failed to find recent {entity}";

        public const string ALL_FAILURE = "Failed to find all {entity}";

        public const string TRACE_METHOD_HIT = "Method {method} hit for {type}";

        public const string PROPERTY_SET = "Set {property} property to value {value}";

        public const string EXCEPTION_DURING_METHOD = "Exception during method. Correlation Id {correlation}";

        public const string CACHE_RESULT_FOR = "Cache hit {hit} for key {key}";

        public const string APPLICATION_FATAL_EXCEPTION = "Application fatal exception";

    }

}
