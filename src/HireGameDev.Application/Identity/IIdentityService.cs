using HireGameDev.Domain.Enums;

namespace HireGameDev.Application.Identity;

/// <summary>
/// Account and sign-in operations. Implemented in Infrastructure on top of ASP.NET Core Identity;
/// no Identity types cross this boundary. Email confirmation tokens are opaque, URL-safe strings.
/// </summary>
public interface IIdentityService
{
    Task<CreateUserResult> CreateUserAsync(
        string email,
        string password,
        UserRole role,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a user whose email was verified by an external provider:
    /// no password, email already confirmed, external login linked.
    /// </summary>
    Task<CreateUserResult> CreateExternalUserAsync(
        string email,
        UserRole role,
        ExternalLoginDetails externalLogin,
        CancellationToken cancellationToken = default);

    Task<UserAccount?> FindUserByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task<string> GenerateEmailConfirmationTokenAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<IdentityOperationResult> ConfirmEmailAsync(
        Guid userId,
        string token,
        CancellationToken cancellationToken = default);

    Task<SignInOutcome> PasswordSignInAsync(
        string email,
        string password,
        bool rememberMe,
        CancellationToken cancellationToken = default);

    Task<SignInOutcome> ExternalLoginSignInAsync(
        string provider,
        string providerKey,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the external login that the OAuth callback left pending for the current
    /// request, or <c>null</c> when there is none (or it expired).
    /// </summary>
    Task<ExternalLoginDetails?> GetExternalLoginInfoAsync(
        CancellationToken cancellationToken = default);

    Task<IdentityOperationResult> AddExternalLoginAsync(
        Guid userId,
        ExternalLoginDetails externalLogin,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Signs in an existing user without checking credentials. Used after an external
    /// provider has already authenticated the user.
    /// </summary>
    Task SignInAsync(
        Guid userId,
        bool rememberMe,
        CancellationToken cancellationToken = default);

    Task SignOutAsync(
        CancellationToken cancellationToken = default);
}
