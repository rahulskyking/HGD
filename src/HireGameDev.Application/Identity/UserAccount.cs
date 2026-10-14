namespace HireGameDev.Application.Identity;

public sealed record UserAccount(
    Guid Id,
    string Email,
    bool EmailConfirmed);
