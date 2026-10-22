using HireGameDev.Application.Persistence;
using HireGameDev.Domain.Entities;

namespace HireGameDev.Application.Features.StudioProfiles.SaveStudioProfile;

public sealed class SaveStudioProfileHandler(
    IStudioProfileRepository studioProfiles,
    ISkillRepository skills,
    IApplicationUnitOfWork unitOfWork)
{
    public async Task<OperationResult> HandleAsync(
        SaveStudioProfileCommand command,
        CancellationToken cancellationToken = default)
    {
        var engineIds = command.EngineIds.Distinct().ToList();

        if (engineIds.Count > StudioProfile.MaximumEngines)
        {
            return OperationResult.Failure($"Pick at most {StudioProfile.MaximumEngines} engines.");
        }

        var knownEngines = await skills.GetByIdsAsync(engineIds, cancellationToken);

        if (knownEngines.Count != engineIds.Count
            || knownEngines.Any(skill => skill.Category != SkillCategory.Engines))
        {
            return OperationResult.Failure("One or more selected engines no longer exist. Please choose again.");
        }

        var profile = await studioProfiles.GetForProfileEditAsync(command.UserId, cancellationToken);

        if (profile is null)
        {
            profile = new StudioProfile(command.UserId);
            studioProfiles.Add(profile);
        }

        try
        {
            profile.UpdateBasics(
                command.Name,
                command.WebsiteUrl,
                command.City,
                command.Size,
                command.Type,
                command.About,
                command.GstNumber);

            profile.ReplaceEngines(engineIds);
            profile.ReplaceGenres(command.Genres);
            profile.ReplaceCultureTags(command.CultureTags);
            profile.ReplaceGames(command.Games);
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
