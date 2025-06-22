using RaftLabs.ExternalUserService.Models;

namespace RaftLabs.ExternalUserService.Services
{
    public interface IUserApiClient
    {
        Task<User?> GetUserByIdAsync(int userId, CancellationToken cancellationToken = default);
        Task<UserListResponse?> GetUsersPageAsync(int page, CancellationToken cancellationToken = default);
    }
}