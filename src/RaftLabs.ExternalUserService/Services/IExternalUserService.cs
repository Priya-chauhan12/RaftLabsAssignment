using RaftLabs.ExternalUserService.Models;

namespace RaftLabs.ExternalUserService.Services
{
    public interface IExternalUserService
    {
        Task<User?> GetUserByIdAsync(int userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<User>> GetAllUsersAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<User>> GetUsersPageAsync(int page, CancellationToken cancellationToken = default);
    }
}