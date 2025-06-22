using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RaftLabs.ExternalUserService.Configuration;
using RaftLabs.ExternalUserService.Models;
using RaftLabs.ExternalUserService.Services;
using System.Net;
using System.Text.Json;

namespace RaftLabs.ExternalUserService.Clients
{
    public class UserApiClient : IUserApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UserApiClient> _logger;
        private readonly ExternalApiOptions _options;
        private readonly JsonSerializerOptions _jsonOptions;

        public UserApiClient(
            HttpClient httpClient,
            ILogger<UserApiClient> logger,
            IOptions<ExternalApiOptions> options)
        {
            _httpClient = httpClient;
            _logger = logger;
            _options = options.Value;

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            _httpClient.BaseAddress = new Uri(_options.BaseUrl);
            _httpClient.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);
        }

        public async Task<User?> GetUserByIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Fetching user with ID: {UserId}", userId);

                var response = await _httpClient.GetAsync($"users/{userId}", cancellationToken);

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("User with ID {UserId} not found", userId);
                    return null;
                }

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<User>>(content, _jsonOptions);

                _logger.LogInformation("Successfully fetched user with ID: {UserId}", userId);
                return apiResponse?.Data;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed while fetching user {UserId}", userId);
                throw new InvalidOperationException($"Failed to fetch user {userId}: {ex.Message}", ex);
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Request timeout while fetching user {UserId}", userId);
                throw new TimeoutException($"Request timeout while fetching user {userId}", ex);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON deserialization failed for user {UserId}", userId);
                throw new InvalidOperationException($"Failed to deserialize response for user {userId}", ex);
            }
        }

        public async Task<UserListResponse?> GetUsersPageAsync(int page, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Fetching users page: {Page}", page);

                var response = await _httpClient.GetAsync($"users?page={page}", cancellationToken);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                var userListResponse = JsonSerializer.Deserialize<UserListResponse>(content, _jsonOptions);

                _logger.LogInformation(
                    "Successfully fetched page {Page} with {Count} users",
                    page, userListResponse?.Data?.Count ?? 0);

                return userListResponse;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed while fetching users page {Page}", page);
                throw new InvalidOperationException($"Failed to fetch users page {page}: {ex.Message}", ex);
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Request timeout while fetching users page {Page}", page);
                throw new TimeoutException($"Request timeout while fetching users page {page}", ex);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON deserialization failed for users page {Page}", page);
                throw new InvalidOperationException($"Failed to deserialize response for users page {page}", ex);
            }
        }
    }
}