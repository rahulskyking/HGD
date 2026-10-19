using HireGameDev.Application.Features.CandidateProfiles.GetMyCandidateProfile;

namespace HireGameDev.Web.ViewModels.CandidateProfile;

public sealed class CandidateDashboardViewModel
{
    public CandidateProfileDto? Profile { get; init; }

    /// <summary>What would raise the completeness score, in the order it is scored.</summary>
    public IReadOnlyList<string> MissingItems { get; init; } = [];
}
