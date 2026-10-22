using HireGameDev.Application.Persistence;
using HireGameDev.Domain.Entities;

namespace HireGameDev.Application.Features.Jobs.SaveStudioJob;

public sealed class SaveStudioJobHandler(
    IStudioProfileRepository studioProfiles,
    IJobRepository jobs,
    ISkillRepository skills,
    IApplicationUnitOfWork unitOfWork)
{
    public async Task<SaveStudioJobOutcome> HandleAsync(
        SaveStudioJobCommand command,
        CancellationToken cancellationToken = default)
    {
        var studioProfileId = await studioProfiles.GetIdByUserIdAsync(
            command.UserId,
            cancellationToken);

        if (studioProfileId is null)
        {
            return SaveStudioJobOutcome.Failure("Create a studio profile before posting a job.");
        }

        var requiredIds = command.RequiredSkillIds.ToList();
        var niceToHaveIds = command.NiceToHaveSkillIds.ToList();
        var allIds = requiredIds.Concat(niceToHaveIds).ToList();

        if (allIds.Count != allIds.Distinct().Count())
        {
            return SaveStudioJobOutcome.Failure(
                "Each skill can be selected once and cannot be both required and nice-to-have.");
        }

        var knownSkills = await skills.GetByIdsAsync(allIds, cancellationToken);

        if (knownSkills.Count != allIds.Count)
        {
            return SaveStudioJobOutcome.Failure(
                "One or more selected skills are no longer available. Please choose again.");
        }

        var isNew = command.JobId is null;
        var job = isNew
            ? new Job(studioProfileId.Value)
            : await jobs.GetForStudioUpdateAsync(
                studioProfileId.Value,
                command.JobId!.Value,
                cancellationToken);

        if (job is null)
        {
            return SaveStudioJobOutcome.Failure("The job could not be found.");
        }

        try
        {
            job.UpdateDetails(
                command.Title,
                command.Category,
                command.Type,
                command.SeniorityLevel,
                command.MinimumAnnualSalary,
                command.MaximumAnnualSalary,
                command.LocationMode,
                command.City,
                command.Description);
            job.ReplaceSkills(requiredIds, niceToHaveIds);
        }
        catch (ArgumentException)
        {
            return SaveStudioJobOutcome.Failure(
                "Some job details are not valid. Check the form and try again.");
        }
        catch (InvalidOperationException exception)
        {
            return SaveStudioJobOutcome.Failure(exception.Message);
        }

        if (isNew)
        {
            jobs.Add(job);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return SaveStudioJobOutcome.Success(job.Id);
    }
}
