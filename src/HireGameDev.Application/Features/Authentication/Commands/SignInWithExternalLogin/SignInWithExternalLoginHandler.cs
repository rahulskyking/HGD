using HireGameDev.Application.Identity;

namespace HireGameDev.Application.Features.Authentication.Commands.SignInWithExternalLogin;

/// <summary>
/// Completes an external (OAuth) sign-in after the provider redirected back:
/// signs in a linked account, links the login to an existing confirmed account with the
/// same email, or asks for registration when the email is unknown.
/// </summary>
public sealed class SignInWithExternalLoginHandler
{
    private readonly IIdentityService _identityService;

    public SignInWithExternalLoginHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<ExternalSignInResult> HandleAsync(
        CancellationToken cancellationToken = default)
    {
        var externalLogin = await _identityService.GetExternalLoginInfoAsync(cancellationToken);

        if (externalLogin is null)
        {
            return ExternalSignInResult.Failed(
                "Sign-in with the external provider could not be completed. Please try again.");
        }

        var outcome = await _identityService.ExternalLoginSignInAsync(
            externalLogin.Provider,
            externalLogin.ProviderKey,
            cancellationToken);

        switch (outcome)
        {
            case SignInOutcome.Succeeded:
                return ExternalSignInResult.SignedIn();

            case SignInOutcome.LockedOut:
                return ExternalSignInResult.LockedOut();

            case SignInOutcome.EmailNotConfirmed:
                return ExternalSignInResult.Failed(
                    "Please confirm your email address before signing in.");
        }

        // No account is linked to this external identity yet.
        if (string.IsNullOrWhiteSpace(externalLogin.Email))
        {
            return ExternalSignInResult.Failed(
                $"{externalLogin.ProviderDisplayName} did not share an email address with us. " +
                "Please register with your email and a password instead.");
        }

        var existing = await _identityService.FindUserByEmailAsync(
            externalLogin.Email,
            cancellationToken);

        if (existing is null)
        {
            return ExternalSignInResult.RegistrationRequired(externalLogin);
        }

        // Linking is only safe when the account owner has already proven ownership of the
        // email; otherwise an unconfirmed account created by someone else could be taken over.
        if (!existing.EmailConfirmed)
        {
            return ExternalSignInResult.Failed(
                "An account with this email address already exists but has not been confirmed yet. " +
                "Please confirm your email address first, then sign in again.");
        }

        var linked = await _identityService.AddExternalLoginAsync(
            existing.Id,
            externalLogin,
            cancellationToken);

        if (!linked.Succeeded)
        {
            return ExternalSignInResult.Failed(string.Join(" ", linked.Errors));
        }

        await _identityService.SignInAsync(existing.Id, rememberMe: false, cancellationToken);

        return ExternalSignInResult.SignedIn();
    }
}
