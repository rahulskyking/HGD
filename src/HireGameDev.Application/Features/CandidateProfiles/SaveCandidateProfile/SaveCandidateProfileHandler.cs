using HireGameDev.Application.Persistence;
using HireGameDev.Domain.Entities;

namespace HireGameDev.Application.Features.CandidateProfiles.SaveCandidateProfile;

public sealed class SaveCandidateProfileHandler(
    ICandidateProfileRepository candidateProfiles,
    ISkillRepository skills,
    IApplicationUnitOfWork unitOfWork)
{
    public async Task<OperationResult> HandleAsync(
        SaveCandidateProfileCommand command,
        CancellationToken cancellationToken = default)
    {
        var requestedSkillIds = command.SkillIds.Distinct().ToList();

        var knownSkills = await skills.GetByIdsAsync(requestedSkillIds, cancellationToken);

        if (knownSkills.Count != requestedSkillIds.Count)
        {
            return OperationResult.Failure("One or more selected skills no longer exist. Please choose again.");
        }

        var profile = await candidateProfiles.GetForProfileEditAsync(command.UserId, cancellationToken);

        if (profile is null)
        {
            profile = new CandidateProfile(command.UserId);
            candidateProfiles.Add(profile);
        }

        try
        {
            profile.UpdateBasics(
                command.FullName,
                command.Phone,
                command.City,
                command.JobTitle,
                command.SeniorityLevel,
                command.YearsOfExperience,
                command.CurrentAnnualSalary,
                command.ExpectedAnnualSalary,
                command.NoticePeriod,
                command.OpenToRemote,
                command.OpenToWorkStatus,
                command.About);

            profile.ReplaceSkills(requestedSkillIds);
            profile.ReplaceShippedTitles(command.ShippedTitles);
            profile.ReplacePortfolioLinks(command.PortfolioLinks);
        }
        catch (ArgumentException)
        {
            // The form enforces the same rules (shared Domain constants) before posting, so this
            // only happens for a tampered or stale post; no need to echo internals back.
            return OperationResult.Failure("Some of the details are not valid. Check the form and try again.");
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return OperationResult.Success();
    }
}
