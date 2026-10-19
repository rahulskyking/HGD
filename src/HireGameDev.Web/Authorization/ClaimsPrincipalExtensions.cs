using System.Security.Claims;
using HireGameDev.Domain.Enums;

namespace HireGameDev.Web.Authorization;

public static class ClaimsPrincipalExtensions
{
    public static bool IsInRole(this ClaimsPrincipal user, UserRole role) =>
        user.IsInRole(role.ToString());

    /// <summary>
    /// The account id Identity put in the sign-in cookie. Only call from authorized actions;
    /// an anonymous principal has no id and this throws rather than returning a guessable default.
    /// </summary>
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.NameIdentifier);

        return Guid.TryParse(value, out var id)
            ? id
            : throw new InvalidOperationException("The signed-in user has no account id claim.");
    }

    /// <summary>
    /// The route for a signed-in user's home, or <c>null</c> for anonymous users
    /// (and roles that do not yet have a home). The single place that maps role → route.
    /// </summary>
    public static string? HomePath(this ClaimsPrincipal user)
    {
        if (user.IsInRole(UserRole.Candidate))
        {
            return "/Candidate";
        }

        if (user.IsInRole(UserRole.Studio))
        {
            return "/Studio";
        }

        return null;
    }
}
