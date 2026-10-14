using HireGameDev.Application.Identity;
using HireGameDev.Domain.Enums;

namespace HireGameDev.Application.Features.Authentication.Commands.CompleteExternalRegistration;

/// <summary>
/// Creates an account from a pending external login (email already verified by the provider)
/// and signs the new user in.
/// </summary>
public sealed class CompleteExternalRegistrationHandler
{
    private readonly IIdentityService _identityService;

    public CompleteExternalRegistrationHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<IdentityOperationResult> HandleAsync(
        CompleteExternalRegistrationCommand command,
        CancellationToken cancellationToken = default)
    {
        if (!command.Role.CanSelfRegister())
        {
            return IdentityOperationResult.Failure(
                "Please choose whether you are registering as a candidate or a studio.");
        }

        var externalLogin = await _identityService.GetExternalLoginInfoAsync(cancellationToken);

        if (externalLogin is null)
        {
            return IdentityOperationResult.Failure(
                "Your external sign-in session has expired. Please start again.");
        }

        if (string.IsNullOrWhiteSpace(externalLogin.Email))
        {
            return IdentityOperationResult.Failure(
                $"{externalLogin.ProviderDisplayName} did not share an email address with us. " +
                "Please register with your email and a password instead.");
        }

        var existing = await _identityService.FindUserByEmailAsync(
            externalLogin.Email,
            cancellationToken);

        if (existing is not null)
        {
            return IdentityOperationResult.Failure(
                "An account with this email address already exists. Please sign in instead.");
        }

        var created = await _identityService.CreateExternalUserAsync(
            externalLogin.Email,
            command.Role,
            externalLogin,
            cancellationToken);

        if (!created.Succeeded)
        {
            return IdentityOperationResult.Failure(created.Errors);
        }

        await _identityService.SignInAsync(created.UserId, rememberMe: false, cancellationToken);

        return IdentityOperationResult.Success();
    }
}
