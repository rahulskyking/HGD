using HireGameDev.Application.Persistence;
using HireGameDev.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HireGameDev.Application.Features.StudioProfiles.SaveStudioProfile;

public sealed class SaveStudioProfileHandler(IApplicationDbContext dbContext)
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

        var knownEngines = await dbContext.Skills
            .Where(x => engineIds.Contains(x.Id) && x.Category == SkillCategory.Engines)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        if (knownEngines.Count != engineIds.Count)
        {
            return OperationResult.Failure("One or more selected engines no longer exist. Please choose again.");
        }

        var profile = await dbContext.StudioProfiles
            .AsSplitQuery()
            .Include(x => x.Engines)
            .Include(x => x.Genres)
            .Include(x => x.CultureTags)
            .Include(x => x.Games)
            .SingleOrDefaultAsync(x => x.UserId == command.UserId, cancellationToken);

        if (profile is null)
        {
            profile = new StudioProfile(command.UserId);
            dbContext.StudioProfiles.Add(profile);
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

        await dbContext.SaveChangesAsync(cancellationToken);

        return OperationResult.Success();
    }
}
