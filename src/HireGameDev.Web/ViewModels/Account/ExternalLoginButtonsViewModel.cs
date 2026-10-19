using HireGameDev.Domain.Enums;

namespace HireGameDev.Web.ViewModels.Account;

/// <summary>Role to assign when creating a new account; null for ordinary sign-in.</summary>
public sealed record ExternalLoginButtonsViewModel(UserRole? Role, string? ReturnUrl);
