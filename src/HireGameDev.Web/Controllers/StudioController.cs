using HireGameDev.Application.Features.StudioProfiles.GetMyStudioProfile;
using HireGameDev.Web.Authorization;
using HireGameDev.Web.ViewModels.StudioProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HireGameDev.Web.Controllers;

[Authorize(Policy = RolePolicies.Studio)]
public sealed class StudioController(GetMyStudioProfileHandler getProfile) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var profile = await getProfile.HandleAsync(
            new GetMyStudioProfileQuery(User.GetUserId()),
            cancellationToken);

        var missingItems = new List<(string Label, string Fragment)>();

        if (profile is not null)
        {
            if (profile.LogoUrl is null)
            {
                missingItems.Add(("Add a studio logo", "logo"));
            }

            if (string.IsNullOrWhiteSpace(profile.About))
            {
                missingItems.Add(("Describe your studio", "basics"));
            }

            if (profile.WebsiteUrl is null)
            {
                missingItems.Add(("Add your website", "basics"));
            }

            if (profile.Engines.Count == 0 && profile.Genres.Count == 0)
            {
                missingItems.Add(("Add engines or genres", "engines"));
            }

            if (profile.Games.Count == 0)
            {
                missingItems.Add(("List a released game", "games"));
            }

            if (profile.CultureTags.Count == 0)
            {
                missingItems.Add(("Share your team culture", "culture"));
            }
        }

        return View(new StudioDashboardViewModel
        {
            Profile = profile,
            MissingItems = missingItems,
        });
    }
}
