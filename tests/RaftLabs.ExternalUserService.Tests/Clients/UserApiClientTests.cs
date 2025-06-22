using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using RaftLabs.ExternalUserService.Clients;
using RaftLabs.ExternalUserService.Configuration;
using System.Net;
using System.Text;
using Xunit;

namespace RaftLabs.ExternalUserService.Tests.Clients
{
    public class UserApiClientTests : IDisposable
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly Mock<ILogger<UserApiClient>> _loggerMock;
        private readonly HttpClient _httpClient;
        private readonly UserApiClient _userApiClient;
        private readonly ExternalApiOptions _options;
        private bool disposedValue;

        public UserApiClientTests()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _loggerMock = new Mock<ILogger<UserApiClient>>();
            _options = new ExternalApiOptions();

            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _userApiClient = new UserApiClient(
                _httpClient,
                _loggerMock.Object,
                Options.Create(_options));
        }

        [Fact]
        public async Task GetUserByIdAsync_ValidUser_ReturnsUser()
        {
            // Arrange
            var userId = 1;
            var jsonResponse = """
                {
                    "data": {
                        "id": 1,
                        "email": "george.bluth@reqres.in",
                        "first_name": "George",
                        "last_name": "Bluth",
                        "avatar": "https://reqres.in/img/faces/1-image.jpg"
                    }
                }
                """;

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
                });

            // Act
            var result = await _userApiClient.GetUserByIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("george.bluth@reqres.in", result.Email);
            Assert.Equal("George", result.FirstName);
            Assert.Equal("Bluth", result.LastName);
        }

        [Fact]
        public async Task GetUserByIdAsync_UserNotFound_ReturnsNull()
        {
            // Arrange
            var userId = 999;

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound
                });

            // Act
            var result = await _userApiClient.GetUserByIdAsync(userId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetUsersPageAsync_ValidPage_ReturnsUsers()
        {
            // Arrange
            var page = 1;
            var jsonResponse = """
                {
                    "page": 1,
                    "per_page": 6,                    
                    "total": 12,
                    "total_pages": 2,
                    "data": [
                        {
                            "id": 1,
                            "email": "george.bluth@reqres.in",
                            "first_name": "George",
                            "last_name": "Bluth",
                            "avatar": "https://reqres.in/img/faces/1-image.jpg"
                        }
                    ]
                }
                """;

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
                });

            // Act
            var result = await _userApiClient.GetUsersPageAsync(page);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Page);
            Assert.Equal(12, result.Total);
            Assert.Equal(2, result.TotalPages);
            Assert.Single(result.Data);
        }

        [Fact]
        public async Task GetUserByIdAsync_HttpRequestException_ThrowsInvalidOperationException()
        {
            // Arrange
            var userId = 1;

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("Network error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _userApiClient.GetUserByIdAsync(userId));

            Assert.Contains("Failed to fetch user", exception.Message);
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

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~UserApiClientTests()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}