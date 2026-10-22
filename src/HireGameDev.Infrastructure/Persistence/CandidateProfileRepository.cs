using HireGameDev.Application.Persistence;
using HireGameDev.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HireGameDev.Infrastructure.Persistence;

public sealed class CandidateProfileRepository(ApplicationDbContext dbContext)
    : ICandidateProfileRepository
{
    public async Task<CandidateProfile?> GetForDisplayAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.CandidateProfiles
            .AsNoTracking()
            .AsSplitQuery()
            .Include(profile => profile.CandidateSkills)
                .ThenInclude(candidateSkill => candidateSkill.Skill)
            .Include(profile => profile.ShippedTitles)
            .Include(profile => profile.PortfolioLinks)
            .SingleOrDefaultAsync(profile => profile.UserId == userId, cancellationToken);
    }

    public async Task<CandidateProfile?> GetForProfileEditAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.CandidateProfiles
            .AsSplitQuery()
            .Include(profile => profile.CandidateSkills)
            .Include(profile => profile.ShippedTitles)
            .Include(profile => profile.PortfolioLinks)
            .SingleOrDefaultAsync(profile => profile.UserId == userId, cancellationToken);
    }

    public async Task<CandidateProfile?> GetForMediaUpdateAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.CandidateProfiles
            .SingleOrDefaultAsync(profile => profile.UserId == userId, cancellationToken);
    }

    public void Add(CandidateProfile profile) => dbContext.CandidateProfiles.Add(profile);
}
