using HireGameDev.Application.Features.CandidateProfiles.GetMyCandidateProfile;
using HireGameDev.Web.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HireGameDev.Web.Pages.Candidate.Profile;

/// <summary>The candidate's own profile as studios will see it (the public page comes with Step 8).</summary>
public class IndexModel(GetMyCandidateProfileHandler getProfile) : PageModel
{
    public CandidateProfileDto Profile { get; private set; } = null!;

    public IReadOnlyList<IGrouping<string, ProfileSkillDto>> SkillGroups { get; private set; } = [];

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        var profile = await getProfile.HandleAsync(new GetMyCandidateProfileQuery(User.GetUserId()), cancellationToken);

        if (profile is null)
        {
            return RedirectToPage("./Edit");
        }

        Profile = profile;
        SkillGroups = profile.Skills.GroupBy(x => x.Category).ToList();

        return Page();
    }
}
