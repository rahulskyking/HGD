using HireGameDev.Application.Features.CandidateProfiles.GetMyCandidateProfile;

namespace HireGameDev.Web.ViewModels.CandidateProfile;

public sealed class CandidateProfileViewModel
{
    public CandidateProfileDto Profile { get; init; } = null!;

    public IReadOnlyList<IGrouping<string, ProfileSkillDto>> SkillGroups { get; init; } = [];

    public string? StatusMessage { get; init; }
}
