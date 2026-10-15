using HireGameDev.Application.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HireGameDev.Application.Features.CandidateProfiles.GetSkills;

public sealed class GetSkillsHandler(IApplicationDbContext dbContext)
{
    public async Task<IReadOnlyList<SkillDto>> HandleAsync(
        GetSkillsQuery query,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Skills
            .AsNoTracking()
            .OrderBy(x => x.Category)
            .ThenBy(x => x.Name)
            .Select(x => new SkillDto(x.Id, x.Name, x.Category))
            .ToListAsync(cancellationToken);
    }
}
