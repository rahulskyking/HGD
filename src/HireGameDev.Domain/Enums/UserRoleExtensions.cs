namespace HireGameDev.Domain.Enums;

public static class UserRoleExtensions
{
    /// <summary>
    /// Only candidates and studios may create their own account.
    /// Admin accounts are provisioned by the platform, never through sign-up.
    /// </summary>
    public static bool CanSelfRegister(this UserRole role) =>
        role is UserRole.Candidate or UserRole.Studio;
}
