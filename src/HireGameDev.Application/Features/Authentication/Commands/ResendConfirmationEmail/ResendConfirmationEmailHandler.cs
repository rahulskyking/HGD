using HireGameDev.Application.Email;
using HireGameDev.Application.Identity;

namespace HireGameDev.Application.Features.Authentication.Commands.ResendConfirmationEmail;

/// <summary>
/// Sends a fresh confirmation link to an unconfirmed account.
/// Always completes silently so the endpoint cannot be used to discover registered emails.
/// </summary>
public sealed class ResendConfirmationEmailHandler
{
    private readonly IIdentityService _identityService;
    private readonly IEmailSender _emailSender;

    public ResendConfirmationEmailHandler(
        IIdentityService identityService,
        IEmailSender emailSender)
    {
        _identityService = identityService;
        _emailSender = emailSender;
    }

    public async Task HandleAsync(
        ResendConfirmationEmailCommand command,
        CancellationToken cancellationToken = default)
    {
        var account = await _identityService.FindUserByEmailAsync(
            command.Email,
            cancellationToken);

        if (account is null || account.EmailConfirmed)
        {
            return;
        }

        var token = await _identityService.GenerateEmailConfirmationTokenAsync(
            account.Id,
            cancellationToken);

        var link = ConfirmationEmail.BuildLink(command.ConfirmEmailUrl, account.Id, token);

        await _emailSender.SendAsync(
            ConfirmationEmail.Create(account.Email, link),
            cancellationToken);
    }
}
