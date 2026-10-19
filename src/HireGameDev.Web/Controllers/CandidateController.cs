using HireGameDev.Application.Features.CandidateProfiles.GetMyCandidateProfile;
using HireGameDev.Web.Authorization;
using HireGameDev.Web.ViewModels.CandidateProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HireGameDev.Web.Controllers;

[Authorize(Policy = RolePolicies.Candidate)]
public sealed class CandidateController(GetMyCandidateProfileHandler getProfile) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var profile = await getProfile.HandleAsync(
            new GetMyCandidateProfileQuery(User.GetUserId()),
            cancellationToken);

        var missingItems = new List<string>();

        if (profile is not null)
        {
            if (profile.PhotoUrl is null)
            {
                missingItems.Add("Add a photo");
            }

            if (profile.About is null)
            {
                missingItems.Add("Write a short About");
            }

            if (profile.ShippedTitles.Count == 0)
            {
                missingItems.Add("Add a shipped title");
            }

            if (profile.PortfolioLinks.Count == 0)
            {
                missingItems.Add("Link your portfolio");
            }
        }

        return View(new CandidateDashboardViewModel
        {
            Profile = profile,
            MissingItems = missingItems,
        });
    }
}
