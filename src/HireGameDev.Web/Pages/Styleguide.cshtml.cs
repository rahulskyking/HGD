using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HireGameDev.Web.Pages;

/// <summary>
/// Living reference for the visual theme (ADR-015): tokens, type, buttons, forms, badges, cards.
/// A developer tool, so it only exists in Development; everywhere else it is a 404.
/// </summary>
public class StyleguideModel : PageModel
{
    private readonly IWebHostEnvironment _environment;

    public StyleguideModel(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public IActionResult OnGet()
    {
        return _environment.IsDevelopment() ? Page() : NotFound();
    }
}
