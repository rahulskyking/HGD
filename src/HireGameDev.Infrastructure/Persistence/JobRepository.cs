using HireGameDev.Application.Persistence;
using HireGameDev.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HireGameDev.Infrastructure.Persistence;

public sealed class JobRepository(ApplicationDbContext dbContext) : IJobRepository
{
    public async Task<IReadOnlyList<Job>> GetForStudioListAsync(
        Guid studioProfileId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Jobs
            .AsNoTracking()
            .Include(job => job.JobSkills)
            .Where(job => job.StudioProfileId == studioProfileId)
            .OrderByDescending(job => job.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<Job?> GetForStudioEditAsync(
        Guid studioProfileId,
        Guid jobId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Jobs
            .AsNoTracking()
            .AsSplitQuery()
            .Include(job => job.JobSkills)
                .ThenInclude(jobSkill => jobSkill.Skill)
            .SingleOrDefaultAsync(
                job => job.Id == jobId && job.StudioProfileId == studioProfileId,
                cancellationToken);
    }

    public async Task<Job?> GetForStudioUpdateAsync(
        Guid studioProfileId,
        Guid jobId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Jobs
            .Include(job => job.JobSkills)
            .SingleOrDefaultAsync(
                job => job.Id == jobId && job.StudioProfileId == studioProfileId,
                cancellationToken);
    }

    public void Add(Job job) => dbContext.Jobs.Add(job);
}
