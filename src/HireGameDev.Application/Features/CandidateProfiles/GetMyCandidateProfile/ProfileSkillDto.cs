namespace HireGameDev.Application.Features.CandidateProfiles.GetMyCandidateProfile;

public sealed record ProfileSkillDto(
    Guid SkillId,
    string Name,
    string Category);
