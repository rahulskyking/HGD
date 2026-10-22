using HireGameDev.Application.Persistence;

namespace HireGameDev.Application.Features.Jobs.GetStudioJob;

public sealed class GetStudioJobHandler(
    IStudioProfileRepository studioProfiles,
    IJobRepository jobs)
{
    public async Task<StudioJobDto?> HandleAsync(
        GetStudioJobQuery query,
        CancellationToken cancellationToken = default)
    {
        var studioProfileId = await studioProfiles.GetIdByUserIdAsync(
            query.UserId,
            cancellationToken);

        if (studioProfileId is null)
        {
            return null;
        }

        var job = await jobs.GetForStudioEditAsync(
            studioProfileId.Value,
            query.JobId,
            cancellationToken);

        if (job is null)
        {
            return null;
        }

        var nowUtc = DateTime.UtcNow;
        var skills = job.JobSkills
            .OrderByDescending(jobSkill => jobSkill.IsRequired)
            .ThenBy(jobSkill => jobSkill.Skill.Name)
            .Select(jobSkill => new StudioJobSkillDto(
                jobSkill.SkillId,
                jobSkill.Skill.Name,
                jobSkill.Skill.Category,
                jobSkill.IsRequired))
            .ToList();

        return new StudioJobDto(
            job.Id,
            job.Title,
            job.Category,
            job.Type,
            job.SeniorityLevel,
            job.MinimumAnnualSalary,
            job.MaximumAnnualSalary,
            job.City,
            job.Description,
            job.LocationMode,
            job.EffectiveStatusAt(nowUtc),
            job.ExpiresAtUtc,
            skills);
    }
}
