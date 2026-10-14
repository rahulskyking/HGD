using HireGameDev.Application.Common;

namespace HireGameDev.Application.Interfaces;

public interface IIdentityService
{
    Task<HireGameDev.Application.Common.IdentityResult> CreateUserAsync(
        string email,
        string password,
        string role,
        CancellationToken cancellationToken = default);

    Task<bool> UserExistsAsync(
        string email,
        CancellationToken cancellationToken = default);
}