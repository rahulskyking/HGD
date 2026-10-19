using HireGameDev.Application.Features.StudioProfiles.GetMyStudioProfile;

namespace HireGameDev.Web.ViewModels.StudioProfile;

public sealed class StudioProfileViewModel
{
    public StudioProfileDto Profile { get; init; } = null!;

    public string? StatusMessage { get; init; }
}
