using HireGameDev.Application.Persistence;
using HireGameDev.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HireGameDev.Infrastructure.Persistence;

public sealed class StudioProfileRepository(ApplicationDbContext dbContext)
    : IStudioProfileRepository
{
    public async Task<StudioProfile?> GetForDisplayAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.StudioProfiles
            .AsNoTracking()
            .AsSplitQuery()
            .Include(profile => profile.Engines)
                .ThenInclude(engine => engine.Skill)
            .Include(profile => profile.Genres)
            .Include(profile => profile.CultureTags)
            .Include(profile => profile.Games)
            .SingleOrDefaultAsync(profile => profile.UserId == userId, cancellationToken);
    }

    public async Task<StudioProfile?> GetForProfileEditAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.StudioProfiles
            .AsSplitQuery()
            .Include(profile => profile.Engines)
            .Include(profile => profile.Genres)
            .Include(profile => profile.CultureTags)
            .Include(profile => profile.Games)
            .SingleOrDefaultAsync(profile => profile.UserId == userId, cancellationToken);
    }

    public async Task<StudioProfile?> GetForMediaUpdateAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.StudioProfiles
            .SingleOrDefaultAsync(profile => profile.UserId == userId, cancellationToken);
    }

    public async Task<Guid?> GetIdByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.StudioProfiles
            .AsNoTracking()
            .Where(profile => profile.UserId == userId)
            .Select(profile => (Guid?)profile.Id)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public void Add(StudioProfile profile) => dbContext.StudioProfiles.Add(profile);
}
