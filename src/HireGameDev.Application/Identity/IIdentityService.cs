using HireGameDev.Domain.Enums;

namespace HireGameDev.Application.Identity;

/// <summary>
/// Account and sign-in operations. Implemented in Infrastructure on top of ASP.NET Core Identity;
/// no Identity types cross this boundary. Accounts are only ever created from an identity that an
/// external provider (Google, GitHub) has verified, so there are no passwords and no email tokens.
/// </summary>
public interface IIdentityService
{
    /// <summary>
    /// Returns the external login that the OAuth callback left pending for the current
    /// request, or <c>null</c> when there is none (or it expired).
    /// </summary>
    Task<ExternalLoginDetails?> GetExternalLoginInfoAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Signs in the account linked to this external login. Fails when no account is linked;
    /// lockout and confirmed-email rules are applied by Identity.
    /// </summary>
    Task<SignInOutcome> ExternalLoginSignInAsync(
        string provider,
        string providerKey,
        CancellationToken cancellationToken = default);

    Task<UserAccount?> FindUserByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a user whose email was verified by an external provider:
    /// no password, email already confirmed, external login linked.
    /// </summary>
    Task<OperationResult> CreateExternalUserAsync(
        string email,
        UserRole role,
        ExternalLoginDetails externalLogin,
        CancellationToken cancellationToken = default);

    Task<OperationResult> AddExternalLoginAsync(
        Guid userId,
        ExternalLoginDetails externalLogin,
        CancellationToken cancellationToken = default);

    Task SignOutAsync(
        CancellationToken cancellationToken = default);
}
