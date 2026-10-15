using HireGameDev.Domain.Enums;

namespace HireGameDev.Application.Features.Authentication.SignInWithExternalLogin;

/// <summary>
/// Completes a Google / GitHub sign-in after the provider redirected back.
/// </summary>
/// <param name="RoleForNewAccount">
/// Role to create the account with when the external identity is unknown — the user chose it
/// before being sent to the provider, or on the follow-up role page. When <c>null</c> and no
/// account exists, the result is <see cref="ExternalSignInStatus.RegistrationRequired"/>.
/// </param>
public sealed record SignInWithExternalLoginCommand(UserRole? RoleForNewAccount);
