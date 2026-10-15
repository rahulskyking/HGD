using HireGameDev.Application.Persistence;
using HireGameDev.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HireGameDev.Application.Features.CandidateProfiles.SaveCandidateProfile;

public sealed class SaveCandidateProfileHandler(IApplicationDbContext dbContext)
{
    public async Task<OperationResult> HandleAsync(
        SaveCandidateProfileCommand command,
        CancellationToken cancellationToken = default)
    {
        var requestedSkillIds = command.SkillIds.Distinct().ToList();

        var knownSkillCount = await dbContext.Skills
            .CountAsync(x => requestedSkillIds.Contains(x.Id), cancellationToken);

        if (knownSkillCount != requestedSkillIds.Count)
        {
            return OperationResult.Failure("One or more selected skills no longer exist. Please choose again.");
        }

        var profile = await dbContext.CandidateProfiles
            .Include(x => x.CandidateSkills)
            .Include(x => x.ShippedTitles)
            .Include(x => x.PortfolioLinks)
            .SingleOrDefaultAsync(x => x.UserId == command.UserId, cancellationToken);

        if (profile is null)
        {
            profile = new CandidateProfile(command.UserId);
            dbContext.CandidateProfiles.Add(profile);
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

        await dbContext.SaveChangesAsync(cancellationToken);

        return OperationResult.Success();
    }
}
