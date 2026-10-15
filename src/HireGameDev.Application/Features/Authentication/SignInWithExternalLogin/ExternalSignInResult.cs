using HireGameDev.Application.Identity;

namespace HireGameDev.Application.Features.Authentication.SignInWithExternalLogin;

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
