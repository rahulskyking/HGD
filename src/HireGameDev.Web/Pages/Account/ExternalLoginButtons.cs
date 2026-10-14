using HireGameDev.Domain.Enums;

namespace HireGameDev.Web.Pages.Account;

/// <summary>Model for <c>_ExternalLoginButtonsPartial</c>.</summary>
/// <param name="Role">Role to create the account with if the user is new; <c>null</c> on plain sign-in.</param>
/// <param name="ReturnUrl">Where to go after sign-in.</param>
public sealed record ExternalLoginButtons(UserRole? Role, string? ReturnUrl);
