using HireGameDev.Application.Features.StudioProfiles.GetMyStudioProfile;
using HireGameDev.Web.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HireGameDev.Web.Pages.Studio.Profile;

/// <summary>The signed-in studio's profile (the public company page comes with Step 7).</summary>
public class IndexModel(GetMyStudioProfileHandler getProfile) : PageModel
{
    public StudioProfileDto Profile { get; private set; } = null!;

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        var profile = await getProfile.HandleAsync(
            new GetMyStudioProfileQuery(User.GetUserId()), cancellationToken);

        if (profile is null)
        {
            return RedirectToPage("./Edit");
        }

        Profile = profile;
        return Page();
    }
}
