using HireGameDev.Domain.Enums;

namespace HireGameDev.Web.ViewModels.Account;

public sealed class SignUpViewModel
{
    public UserRole? Role { get; init; }

    public string? ReturnUrl { get; init; }
}
