using HireGameDev.Application.Features.StudioProfiles.GetMyStudioProfile;

namespace HireGameDev.Web.ViewModels.StudioProfile;

public sealed class StudioDashboardViewModel
{
    public StudioProfileDto? Profile { get; init; }

    public IReadOnlyList<(string Label, string Fragment)> MissingItems { get; init; } = [];
}
