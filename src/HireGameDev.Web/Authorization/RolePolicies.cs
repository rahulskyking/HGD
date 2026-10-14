using HireGameDev.Domain.Enums;

namespace HireGameDev.Web.Authorization;

/// <summary>
/// One authorization policy per self-service role; the policy name is the role name.
/// Applied per folder in Program.cs (/Candidate, /Studio) so every page there is protected by default.
/// </summary>
public static class RolePolicies
{
    public const string Candidate = nameof(UserRole.Candidate);
    public const string Studio = nameof(UserRole.Studio);

    public static IServiceCollection AddRolePolicies(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy(Candidate, policy => policy.RequireRole(Candidate))
            .AddPolicy(Studio, policy => policy.RequireRole(Studio));

        return services;
    }
}
