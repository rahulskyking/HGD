using HireGameDev.Application.Features.CandidateProfiles.GetMyCandidateProfile;
using HireGameDev.Web.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HireGameDev.Web.Pages.Candidate;

// Protected by the /Candidate folder convention in DependencyInjection.cs (Candidate role required).
public class IndexModel(GetMyCandidateProfileHandler getProfile) : PageModel
{
    public CandidateProfileDto? Profile { get; private set; }

    /// <summary>What would raise the completeness score, in the order it is scored.</summary>
    public IReadOnlyList<string> MissingItems { get; private set; } = [];

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Profile = await getProfile.HandleAsync(new GetMyCandidateProfileQuery(User.GetUserId()), cancellationToken);

        if (Profile is null)
        {
            return;
        }

        var missing = new List<string>();

        if (Profile.PhotoUrl is null)
        {
            missing.Add("Add a photo");
        }

        if (Profile.About is null)
        {
            missing.Add("Write a short About");
        }

        if (Profile.ShippedTitles.Count == 0)
        {
            missing.Add("Add a shipped title");
        }

        if (Profile.PortfolioLinks.Count == 0)
        {
            missing.Add("Link your portfolio");
        }

        MissingItems = missing;
    }
}
