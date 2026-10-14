using HireGameDev.Application.Identity;

namespace HireGameDev.Application.Features.Authentication.Commands.SignInWithExternalLogin;

public enum ExternalSignInStatus
{
    SignedIn,

    LockedOut,

    /// <summary>No account exists for this external identity yet; the user must pick a role to create one.</summary>
    RegistrationRequired,

    Failed
}

public sealed record ExternalSignInResult(
    ExternalSignInStatus Status,
    ExternalLoginDetails? ExternalLogin,
    string? Error)
{
    public static ExternalSignInResult SignedIn() =>
        new(ExternalSignInStatus.SignedIn, null, null);

    public static ExternalSignInResult LockedOut() =>
        new(ExternalSignInStatus.LockedOut, null, null);

    public static ExternalSignInResult RegistrationRequired(ExternalLoginDetails externalLogin) =>
        new(ExternalSignInStatus.RegistrationRequired, externalLogin, null);

    public static ExternalSignInResult Failed(string error) =>
        new(ExternalSignInStatus.Failed, null, error);
}
