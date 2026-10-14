using HireGameDev.Application.Identity;

namespace HireGameDev.Application.Features.Authentication.Commands.SignInWithExternalLogin;

public enum ExternalSignInStatus
{
    SignedIn,

    /// <summary>No account exists for this external identity and no role was given; ask the user which role to create.</summary>
    RegistrationRequired,

    /// <summary>Could not sign in; <see cref="ExternalSignInResult.Error"/> explains why in user-facing terms.</summary>
    Failed
}

public sealed record ExternalSignInResult(
    ExternalSignInStatus Status,
    ExternalLoginDetails? ExternalLogin,
    string? Error)
{
    public static ExternalSignInResult SignedIn() =>
        new(ExternalSignInStatus.SignedIn, null, null);

    public static ExternalSignInResult RegistrationRequired(ExternalLoginDetails externalLogin) =>
        new(ExternalSignInStatus.RegistrationRequired, externalLogin, null);

    public static ExternalSignInResult Failed(string error) =>
        new(ExternalSignInStatus.Failed, null, error);
}
