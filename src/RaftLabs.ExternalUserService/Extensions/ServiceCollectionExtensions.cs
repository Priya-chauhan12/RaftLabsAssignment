using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using RaftLabs.ExternalUserService.Clients;
using RaftLabs.ExternalUserService.Configuration;
using RaftLabs.ExternalUserService.Services;

namespace RaftLabs.ExternalUserService.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddExternalUserService(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Configure options
            services.Configure<ExternalApiOptions>(
                configuration.GetSection(ExternalApiOptions.SectionName));

            // Add memory cache
            services.AddMemoryCache();

            // Add HTTP client with Polly retry policy
            services.AddHttpClient<IUserApiClient, UserApiClient>(client =>
            {
                var options = configuration.GetSection(ExternalApiOptions.SectionName)
                    .Get<ExternalApiOptions>() ?? new ExternalApiOptions();

                client.BaseAddress = new Uri(options.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
            })
            .AddPolicyHandler(GetRetryPolicy());

            // Register services
            services.AddScoped<IExternalUserService, Services.ExternalUserService>();

            return services;
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError() // Handles HttpRequestException and 5XX, 408 status codes
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), // Exponential backoff
                    onRetry: (outcome, duration, retryCount, context) =>
                    {

                        // You can log using a static logger if you want, or just skip
                        Console.WriteLine($"Retry {retryCount} after {duration.TotalMilliseconds}ms");

                    });
        }
    }
}