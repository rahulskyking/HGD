using HireGameDev.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HireGameDev.Web.Pages.Account;

/// <summary>
/// Step 1 of joining: pick a role, then continue with Google or GitHub.
/// Without a role the page shows the two choices; with one it shows the provider buttons.
/// </summary>
[AllowAnonymous]
public class SignUpModel : PageModel
{
    public UserRole? Role { get; private set; }

    public string? ReturnUrl { get; private set; }

    public IActionResult OnGet(UserRole? role = null, string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToPage("/Index");
        }

        // Only self-service roles can be chosen here; anything else falls back to the choice.
        Role = role is { } chosen && chosen.CanSelfRegister() ? chosen : null;
        ReturnUrl = returnUrl;

        return Page();
    }
}
