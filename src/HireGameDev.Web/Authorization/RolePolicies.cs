using HireGameDev.Domain.Enums;

namespace HireGameDev.Web.Authorization;

/// <summary>
/// Names of the authorization policies, one per self-service role (the policy name is the role name).
/// Registered in <see cref="DependencyInjection.AddWeb"/> and applied per folder (/Candidate, /Studio)
/// so every page there is protected by default; use <c>[Authorize(Policy = RolePolicies.Studio)]</c>
/// for a single page elsewhere.
/// </summary>
public static class RolePolicies
{
    public const string Candidate = nameof(UserRole.Candidate);
    public const string Studio = nameof(UserRole.Studio);
}
