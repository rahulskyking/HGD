using HireGameDev.Application.Email;
using HireGameDev.Application.Identity;
using HireGameDev.Domain.Enums;

namespace HireGameDev.Application.Features.Authentication.Commands.RegisterUser;

/// <summary>
/// Creates a password-based account and sends the email confirmation link.
/// The account cannot sign in until the email is confirmed.
/// </summary>
public sealed class RegisterUserHandler
{
    private readonly IIdentityService _identityService;
    private readonly IEmailSender _emailSender;

    public RegisterUserHandler(
        IIdentityService identityService,
        IEmailSender emailSender)
    {
        _identityService = identityService;
        _emailSender = emailSender;
    }

    public async Task<IdentityOperationResult> HandleAsync(
        RegisterUserCommand command,
        CancellationToken cancellationToken = default)
    {
        if (!command.Role.CanSelfRegister())
        {
            return IdentityOperationResult.Failure(
                "Please choose whether you are registering as a candidate or a studio.");
        }

        var created = await _identityService.CreateUserAsync(
            command.Email,
            command.Password,
            command.Role,
            cancellationToken);

        if (!created.Succeeded)
        {
            return IdentityOperationResult.Failure(created.Errors);
        }

        var token = await _identityService.GenerateEmailConfirmationTokenAsync(
            created.UserId,
            cancellationToken);

        var link = ConfirmationEmail.BuildLink(command.ConfirmEmailUrl, created.UserId, token);

        await _emailSender.SendAsync(
            ConfirmationEmail.Create(command.Email, link),
            cancellationToken);

        return IdentityOperationResult.Success();
    }
}
