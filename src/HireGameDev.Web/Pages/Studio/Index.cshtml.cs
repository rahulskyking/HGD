using HireGameDev.Application.Features.StudioProfiles.GetMyStudioProfile;
using HireGameDev.Web.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HireGameDev.Web.Pages.Studio;

// Protected by the /Studio folder convention in Program.cs (Studio role required).
public class IndexModel(GetMyStudioProfileHandler getProfile) : PageModel
{
    public StudioProfileDto? Profile { get; private set; }

    public IReadOnlyList<(string Label, string Fragment)> MissingItems { get; private set; } = [];

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Profile = await getProfile.HandleAsync(
            new GetMyStudioProfileQuery(User.GetUserId()), cancellationToken);

        if (Profile is null)
        {
            return;
        }

        var missing = new List<(string Label, string Fragment)>();

        if (Profile.LogoUrl is null)
        {
            missing.Add(("Add a studio logo", "logo"));
        }

        if (string.IsNullOrWhiteSpace(Profile.About))
        {
            missing.Add(("Describe your studio", "basics"));
        }

        if (Profile.WebsiteUrl is null)
        {
            missing.Add(("Add your website", "basics"));
        }

        if (Profile.Engines.Count == 0 && Profile.Genres.Count == 0)
        {
            missing.Add(("Add engines or genres", "engines"));
        }

        if (Profile.Games.Count == 0)
        {
            missing.Add(("List a released game", "games"));
        }

        if (Profile.CultureTags.Count == 0)
        {
            missing.Add(("Share your team culture", "culture"));
        }

        MissingItems = missing;
    }
}
