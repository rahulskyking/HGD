using HireGameDev.Web.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HireGameDev.Web.Pages;

public class IndexModel : PageModel
{
    // Signed-in users go straight to their role's home; everyone else sees the landing page.
    public IActionResult OnGet() =>
        User.HomePage() is { } home
            ? RedirectToPage(home)
            : Page();
}
