namespace RaftLabs.ExternalUserService.Configuration
{
    public class ExternalApiOptions
    {
        public const string SectionName = "ExternalApi";

        public string BaseUrl { get; set; } = "https://reqres.in/api/";
        public int TimeoutSeconds { get; set; } = 30;
        public int MaxRetryAttempts { get; set; } = 3;
        public int CacheExpirationMinutes { get; set; } = 10;
    }
}