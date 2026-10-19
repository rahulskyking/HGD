using HireGameDev.Application.Features.Skills.GetSkills;
using HireGameDev.Application.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HireGameDev.Application.Features.StudioProfiles.GetMyStudioProfile;

public sealed class GetMyStudioProfileHandler(IApplicationDbContext dbContext)
{
    public async Task<StudioProfileDto?> HandleAsync(
        GetMyStudioProfileQuery query,
        CancellationToken cancellationToken = default)
    {
        var profile = await dbContext.StudioProfiles
            .AsNoTracking()
            .AsSplitQuery()
            .Include(x => x.Engines)
                .ThenInclude(x => x.Skill)
            .Include(x => x.Genres)
            .Include(x => x.CultureTags)
            .Include(x => x.Games)
            .SingleOrDefaultAsync(x => x.UserId == query.UserId, cancellationToken);

        if (profile is null)
        {
            return null;
        }

        return new StudioProfileDto(
            profile.Id,
            profile.Name,
            profile.LogoUrl,
            profile.WebsiteUrl,
            profile.City,
            profile.State,
            profile.Size,
            profile.Type,
            profile.About,
            profile.GstNumber,
            profile.IsVerified,
            profile.Engines
                .Select(x => new SkillDto(x.SkillId, x.Skill.Name, x.Skill.Category))
                .OrderBy(x => x.Name)
                .ToList(),
            profile.Genres
                .Select(x => x.Genre)
                .OrderBy(x => x)
                .ToList(),
            profile.CultureTags
                .Select(x => x.Tag)
                .OrderBy(x => x)
                .ToList(),
            profile.Games
                .Select(x => new StudioGameDto(x.Name, x.Year, x.Url))
                .OrderByDescending(x => x.Year)
                .ThenBy(x => x.Name)
                .ToList(),
            profile.CompletenessPercent(),
            profile.UpdatedAtUtc ?? profile.CreatedAtUtc);
    }
}
