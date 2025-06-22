using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RaftLabs.ExternalUserService.Configuration;
using RaftLabs.ExternalUserService.Models;

namespace RaftLabs.ExternalUserService.Services
{
    public class ExternalUserService : IExternalUserService
    {
        private readonly IUserApiClient _apiClient;
        private readonly IMemoryCache _cache;
        private readonly ILogger<ExternalUserService> _logger;
        private readonly ExternalApiOptions _options;

        public ExternalUserService(
            IUserApiClient apiClient,
            IMemoryCache cache,
            ILogger<ExternalUserService> logger,
            IOptions<ExternalApiOptions> options)
        {
            _apiClient = apiClient;
            _cache = cache;
            _logger = logger;
            _options = options.Value;
        }

        public async Task<User?> GetUserByIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            var cacheKey = $"user_{userId}";

            if (_cache.TryGetValue(cacheKey, out User? cachedUser))
            {
                _logger.LogInformation("Retrieved user {UserId} from cache", userId);
                return cachedUser;
            }

            var user = await _apiClient.GetUserByIdAsync(userId, cancellationToken);

            if (user != null)
            {
                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_options.CacheExpirationMinutes)
                };

                _cache.Set(cacheKey, user, cacheOptions);
                _logger.LogInformation("Cached user {UserId} for {Minutes} minutes", userId, _options.CacheExpirationMinutes);
            }

            return user;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync(CancellationToken cancellationToken = default)
        {
            const string cacheKey = "all_users";

            if (_cache.TryGetValue(cacheKey, out IEnumerable<User>? cachedUsers))
            {
                _logger.LogInformation("Retrieved all users from cache");
                return cachedUsers!;
            }

            var allUsers = new List<User>();
            var page = 1;
            UserListResponse? response;

            do
            {
                response = await _apiClient.GetUsersPageAsync(page, cancellationToken);
                if (response?.Data != null)
                {
                    allUsers.AddRange(response.Data);
                    _logger.LogInformation("Fetched page {Page} with {Count} users", page, response.Data.Count);
                }
                page++;
            }
            while (response != null && page <= response.TotalPages);

            var result = allUsers.AsEnumerable();

            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_options.CacheExpirationMinutes)
            };

            _cache.Set(cacheKey, result, cacheOptions);
            _logger.LogInformation("Cached {Count} users for {Minutes} minutes", allUsers.Count, _options.CacheExpirationMinutes);

            return result;
        }

        public async Task<IEnumerable<User>> GetUsersPageAsync(int page, CancellationToken cancellationToken = default)
        {
            var cacheKey = $"users_page_{page}";

            if (_cache.TryGetValue(cacheKey, out IEnumerable<User>? cachedUsers))
            {
                _logger.LogInformation("Retrieved users page {Page} from cache", page);
                return cachedUsers!;
            }

            var response = await _apiClient.GetUsersPageAsync(page, cancellationToken);
            var users = response?.Data ?? new List<User>();

            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_options.CacheExpirationMinutes)
            };

            _cache.Set(cacheKey, users.AsEnumerable(), cacheOptions);
            _logger.LogInformation("Cached users page {Page} for {Minutes} minutes", page, _options.CacheExpirationMinutes);

            return users;
        }
    }
}