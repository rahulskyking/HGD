using HireGameDev.Domain.Enums;

namespace HireGameDev.Application.Features.Authentication.Commands.RegisterUser;

/// <param name="ConfirmEmailUrl">
/// Absolute URL of the confirm-email endpoint. The user id and confirmation token are appended
/// as query parameters when the confirmation link is built.
/// </param>
public sealed record RegisterUserCommand(
    string Email,
    string Password,
    UserRole Role,
    string ConfirmEmailUrl);
