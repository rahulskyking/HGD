using HireGameDev.Application.Persistence;
using HireGameDev.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HireGameDev.Infrastructure.Persistence;

public sealed class SkillRepository(ApplicationDbContext dbContext)
    : ISkillRepository
{
    public async Task<IReadOnlyList<Skill>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Skills
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Skill>> GetByIdsAsync(
        IReadOnlyCollection<Guid> skillIds,
        CancellationToken cancellationToken = default)
    {
        if (skillIds.Count == 0)
        {
            return Array.Empty<Skill>();
        }

        return await dbContext.Skills
            .AsNoTracking()
            .Where(skill => skillIds.Contains(skill.Id))
            .ToListAsync(cancellationToken);
    }
}
