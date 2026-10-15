using HireGameDev.Application.Identity;
using HireGameDev.Domain.Enums;

namespace HireGameDev.Application.Features.Authentication.SignInWithExternalLogin;

/// <summary>
/// The whole sign-in / sign-up path for external providers, in order:
/// sign in a linked account; otherwise link the login to an existing confirmed account with
/// the same email; otherwise create an account with the chosen role (or ask for one).
/// </summary>
public sealed class SignInWithExternalLoginHandler
{
    private const string GenericError = "Sign-in could not be completed. Please try again.";

    private readonly IIdentityService _identityService;

    public SignInWithExternalLoginHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<ExternalSignInResult> HandleAsync(
        SignInWithExternalLoginCommand command,
        CancellationToken cancellationToken = default)
    {
        var externalLogin = await _identityService.GetExternalLoginInfoAsync(cancellationToken);

        if (externalLogin is null)
        {
            return ExternalSignInResult.Failed(
                "Your sign-in session has expired. Please try again.");
        }

        // 1. An account is already linked to this login.
        var outcome = await SignInAsync(externalLogin, cancellationToken);

        if (outcome != SignInOutcome.Failed)
        {
            return ToResult(outcome);
        }

        // 2. Nothing linked: find the account by email, or create one.
        if (string.IsNullOrWhiteSpace(externalLogin.Email))
        {
            return ExternalSignInResult.Failed(
                $"{externalLogin.ProviderDisplayName} did not share an email address with us. " +
                "Please try another sign-in option.");
        }

        var existing = await _identityService.FindUserByEmailAsync(externalLogin.Email, cancellationToken);

        var prepared = existing is null
            ? await CreateAccountAsync(externalLogin, command.RoleForNewAccount, cancellationToken)
            : await LinkToExistingAccountAsync(externalLogin, existing, cancellationToken);

        if (prepared is not null)
        {
            return prepared;
        }

        // 3. The login is linked now; sign in through Identity so lockout rules apply.
        outcome = await SignInAsync(externalLogin, cancellationToken);

        return outcome == SignInOutcome.Failed
            ? ExternalSignInResult.Failed(GenericError)
            : ToResult(outcome);
    }

    /// <summary>Returns <c>null</c> when the account was created and is ready to sign in.</summary>
    private async Task<ExternalSignInResult?> CreateAccountAsync(
        ExternalLoginDetails externalLogin,
        UserRole? role,
        CancellationToken cancellationToken)
    {
        if (role is null)
        {
            return ExternalSignInResult.RegistrationRequired(externalLogin);
        }

        if (!role.Value.CanSelfRegister())
        {
            return ExternalSignInResult.Failed(
                "Please choose whether you are a game developer or a studio.");
        }

        var created = await _identityService.CreateExternalUserAsync(
            externalLogin.Email!,
            role.Value,
            externalLogin,
            cancellationToken);

        return created.Succeeded
            ? null
            : ExternalSignInResult.Failed(string.Join(" ", created.Errors));
    }

    /// <summary>Returns <c>null</c> when the login was linked and the account is ready to sign in.</summary>
    private async Task<ExternalSignInResult?> LinkToExistingAccountAsync(
        ExternalLoginDetails externalLogin,
        UserAccount existing,
        CancellationToken cancellationToken)
    {
        // Linking is only safe when the account owner has proven ownership of the email;
        // otherwise an unconfirmed account created by someone else could be taken over.
        if (!existing.EmailConfirmed)
        {
            return ExternalSignInResult.Failed(
                "An account with this email address exists but was never verified. Please contact support.");
        }

        var linked = await _identityService.AddExternalLoginAsync(existing.Id, externalLogin, cancellationToken);

        return linked.Succeeded
            ? null
            : ExternalSignInResult.Failed(string.Join(" ", linked.Errors));
    }

    private Task<SignInOutcome> SignInAsync(ExternalLoginDetails externalLogin, CancellationToken cancellationToken) =>
        _identityService.ExternalLoginSignInAsync(
            externalLogin.Provider,
            externalLogin.ProviderKey,
            cancellationToken);

    private static ExternalSignInResult ToResult(SignInOutcome outcome) =>
        outcome switch
        {
            SignInOutcome.Succeeded => ExternalSignInResult.SignedIn(),
            SignInOutcome.LockedOut => ExternalSignInResult.Failed(
                "This account is temporarily locked. Please try again later."),
            SignInOutcome.EmailNotConfirmed => ExternalSignInResult.Failed(
                "This account's email address was never verified. Please contact support."),
            _ => ExternalSignInResult.Failed(GenericError)
        };
}
