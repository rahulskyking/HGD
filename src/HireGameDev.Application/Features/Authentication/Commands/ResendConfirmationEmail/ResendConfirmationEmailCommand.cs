namespace HireGameDev.Application.Features.Authentication.Commands.ResendConfirmationEmail;

/// <param name="ConfirmEmailUrl">Absolute URL of the confirm-email endpoint (see RegisterUserCommand).</param>
public sealed record ResendConfirmationEmailCommand(
    string Email,
    string ConfirmEmailUrl);
