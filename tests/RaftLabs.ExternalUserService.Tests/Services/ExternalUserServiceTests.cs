using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RaftLabs.ExternalUserService.Configuration;
using RaftLabs.ExternalUserService.Models;
using RaftLabs.ExternalUserService.Services;
using Xunit;

namespace RaftLabs.ExternalUserService
{
    public class ExternalUserServiceTests : IDisposable
    {
        private readonly Mock<IUserApiClient> _apiClientMock;
        private readonly Mock<ILogger<Services.ExternalUserService>> _loggerMock;
        private readonly IMemoryCache _memoryCache;
        private readonly Services.ExternalUserService _userService;
        private readonly ExternalApiOptions _options;
        private bool disposedValue;

        public ExternalUserServiceTests()
        {
            _apiClientMock = new Mock<IUserApiClient>();
            _loggerMock = new Mock<ILogger<Services.ExternalUserService>>();
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _options = new ExternalApiOptions { CacheExpirationMinutes = 10 };

            _userService = new Services.ExternalUserService(
                _apiClientMock.Object,
                _memoryCache,
                _loggerMock.Object,
                Options.Create(_options));
        }

        [Fact]
        public async Task GetUserByIdAsync_FirstCall_CallsApiAndCachesResult()
        {
            // Arrange
            var userId = 1;
            var user = new User
            {
                Id = 1,
                Email = "test@test.com",
                FirstName = "John",
                LastName = "Doe"
            };

            _apiClientMock
                .Setup(x => x.GetUserByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.GetUserByIdAsync(userId);

            // Assert
            Assert.Equal(user, result);
            _apiClientMock.Verify(x => x.GetUserByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetUserByIdAsync_SecondCall_ReturnsCachedResult()
        {
            // Arrange
            var userId = 1;
            var user = new User
            {
                Id = 1,
                Email = "test@test.com",
                FirstName = "John",
                LastName = "Doe"
            };

            _apiClientMock
                .Setup(x => x.GetUserByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Act
            var firstResult = await _userService.GetUserByIdAsync(userId);
            var secondResult = await _userService.GetUserByIdAsync(userId);

            // Assert
            Assert.Equal(user, firstResult);
            Assert.Equal(user, secondResult);
            _apiClientMock.Verify(x => x.GetUserByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetAllUsersAsync_MultiplePagesExist_FetchesAllPages()
        {
            // Arrange
            var page1Response = new UserListResponse
            {
                Page = 1,
                TotalPages = 2,
                Data = new List<User>
                {
                    new() { Id = 1, Email = "user1@test.com", FirstName = "User", LastName = "One" }
                }
            };

            var page2Response = new UserListResponse
            {
                Page = 2,
                TotalPages = 2,
                Data = new List<User>
                {
                    new() { Id = 2, Email = "user2@test.com", FirstName = "User", LastName = "Two" }
                }
            };

            _apiClientMock
                .Setup(x => x.GetUsersPageAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(page1Response);

            _apiClientMock
                .Setup(x => x.GetUsersPageAsync(2, It.IsAny<CancellationToken>()))
                .ReturnsAsync(page2Response);

            // Act
            var result = await _userService.GetAllUsersAsync();

            // Assert
            Assert.Equal(2, result.Count());
            _apiClientMock.Verify(x => x.GetUsersPageAsync(1, It.IsAny<CancellationToken>()), Times.Once);
            _apiClientMock.Verify(x => x.GetUsersPageAsync(2, It.IsAny<CancellationToken>()), Times.Once);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }



        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}