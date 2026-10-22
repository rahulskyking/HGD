using HireGameDev.Application.Persistence;

namespace HireGameDev.Application.Features.CandidateProfiles.GetMyCandidateProfile;

public sealed class GetMyCandidateProfileHandler(ICandidateProfileRepository candidateProfiles)
{
    public async Task<CandidateProfileDto?> HandleAsync(
        GetMyCandidateProfileQuery query,
        CancellationToken cancellationToken = default)
    {
        var profile = await candidateProfiles.GetForDisplayAsync(query.UserId, cancellationToken);

        if (profile is null)
        {
            return null;
        }

        return new CandidateProfileDto(
            profile.Id,
            profile.FullName,
            profile.PhotoUrl,
            profile.Phone,
            profile.City,
            profile.JobTitle,
            profile.SeniorityLevel,
            profile.YearsOfExperience,
            profile.CurrentAnnualSalary,
            profile.ExpectedAnnualSalary,
            profile.NoticePeriod,
            profile.OpenToRemote,
            profile.OpenToWorkStatus,
            profile.About,
            profile.CandidateSkills
                .Select(x => new ProfileSkillDto(x.SkillId, x.Skill.Name, x.Skill.Category))
                .OrderBy(x => x.Category)
                .ThenBy(x => x.Name)
                .ToList(),
            profile.ShippedTitles
                .Select(x => new ShippedTitleDto(x.GameName, x.StudioName, x.Role, x.Year, x.IsVerified))
                .OrderByDescending(x => x.Year)
                .ThenBy(x => x.GameName)
                .ToList(),
            profile.PortfolioLinks
                .Select(x => new PortfolioLinkDto(x.Url, x.Label))
                .ToList(),
            profile.CompletenessPercent(),
            profile.UpdatedAtUtc ?? profile.CreatedAtUtc);
    }
}
