using HireGameDev.Domain.Enums;

namespace HireGameDev.Application.Features.Authentication.Commands.CompleteExternalRegistration;

/// <summary>
/// The role chosen by a user whose external sign-in had no matching account.
/// The external identity itself is read from the pending external login, never from the form.
/// </summary>
public sealed record CompleteExternalRegistrationCommand(UserRole Role);
