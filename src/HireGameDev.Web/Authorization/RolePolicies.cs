using HireGameDev.Domain.Enums;

namespace HireGameDev.Web.Authorization;

/// <summary>
/// Names of the authorization policies, one per self-service role (the policy name is the role name).
/// Registered in <see cref="DependencyInjection.AddWeb"/> and applied at controller/action level
/// so every MVC action for a role is protected by default; use <c>[Authorize(Policy = RolePolicies.Studio)]</c>
/// for a single controller or action elsewhere.
/// </summary>
public static class RolePolicies
{
    public const string Candidate = nameof(UserRole.Candidate);
    public const string Studio = nameof(UserRole.Studio);
}
