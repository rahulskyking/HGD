using HireGameDev.Application.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HireGameDev.Web.Pages.Account;

public class LogoutModel : PageModel
{
    private readonly IIdentityService _identityService;

    public LogoutModel(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    // Signing out is a state change, so it only happens on POST (with antiforgery).
    public IActionResult OnGet() => RedirectToPage("/Index");

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        await _identityService.SignOutAsync(cancellationToken);

        return RedirectToPage("/Index");
    }
}
