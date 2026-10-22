using HireGameDev.Application.Persistence;

namespace HireGameDev.Application.Features.Jobs.GetStudioJobs;

public sealed class GetStudioJobsHandler(
    IStudioProfileRepository studioProfiles,
    IJobRepository jobs)
{
    public async Task<StudioJobsPageDto> HandleAsync(
        GetStudioJobsQuery query,
        CancellationToken cancellationToken = default)
    {
        var studioProfileId = await studioProfiles.GetIdByUserIdAsync(
            query.UserId,
            cancellationToken);

        if (studioProfileId is null)
        {
            return new StudioJobsPageDto(false, []);
        }

        var nowUtc = DateTime.UtcNow;
        var studioJobs = await jobs.GetForStudioListAsync(studioProfileId.Value, cancellationToken);
        var results = studioJobs
            .Select(job => new StudioJobSummaryDto(
                job.Id,
                job.Title,
                job.Category,
                job.Type,
                job.SeniorityLevel,
                job.MinimumAnnualSalary,
                job.MaximumAnnualSalary,
                job.City,
                job.LocationMode,
                job.EffectiveStatusAt(nowUtc),
                job.ExpiresAtUtc,
                job.CreatedAtUtc,
                job.JobSkills.Count(skill => skill.IsRequired),
                job.JobSkills.Count(skill => !skill.IsRequired)))
            .ToList();

        return new StudioJobsPageDto(true, results);
    }
}
