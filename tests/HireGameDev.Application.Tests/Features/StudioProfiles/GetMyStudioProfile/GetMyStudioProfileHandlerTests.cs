using HireGameDev.Application.Features.StudioProfiles.GetMyStudioProfile;
using HireGameDev.Application.Tests.Persistence;
using HireGameDev.Domain.Entities;
using HireGameDev.Domain.Enums;

namespace HireGameDev.Application.Tests.Features.StudioProfiles.GetMyStudioProfile;

public class GetMyStudioProfileHandlerTests : IDisposable
{
    private readonly SqliteDatabase _database = new();

    public void Dispose() => _database.Dispose();

    [Fact]
    public async Task Returns_null_when_the_studio_has_no_profile_yet()
    {
        var userId = await _database.AddUserAsync();

        await using var context = _database.CreateContext();
        var result = await new GetMyStudioProfileHandler(context)
            .HandleAsync(new GetMyStudioProfileQuery(userId));

        Assert.Null(result);
    }

    [Fact]
    public async Task Maps_a_profile_with_its_engines_genres_culture_and_games()
    {
        var userId = await _database.AddUserAsync();
        var engines = await _database.AddSkillsAsync("Unity", "Unreal Engine 5");
        var profile = new StudioProfile(userId);
        profile.UpdateBasics(
            "Nodding Heads Games", "https://noddingheadsgames.com", "Pune",
            StudioSize.From11To50, StudioType.GameStudio, "We make games.", "29ABCDE1234F1Z5");
        profile.ReplaceEngines(engines.Select(x => x.Id).ToList());
        profile.ReplaceGenres([GameGenre.Rpg, GameGenre.Puzzle]);
        profile.ReplaceCultureTags([CultureTag.NoCrunch]);
        profile.ReplaceGames(
        [
            new StudioGameDetails("Raji: An Ancient Epic", 2020, "https://example.com/raji"),
            new StudioGameDetails("Ludo King", 2023, null),
        ]);

        await using (var setup = _database.CreateContext())
        {
            setup.StudioProfiles.Add(profile);
            await setup.SaveChangesAsync();
        }

        await using var context = _database.CreateContext();
        var result = await new GetMyStudioProfileHandler(context)
            .HandleAsync(new GetMyStudioProfileQuery(userId));

        Assert.NotNull(result);
        Assert.Equal("Pune", result.City);
        Assert.Equal("Maharashtra", result.State);
        Assert.Equal(new[] { "Unity", "Unreal Engine 5" }, result.Engines.Select(x => x.Name));
        Assert.Equal(new[] { GameGenre.Puzzle, GameGenre.Rpg }, result.Genres);
        Assert.Equal(new[] { CultureTag.NoCrunch }, result.CultureTags);
        Assert.Equal(new[] { "Ludo King", "Raji: An Ancient Epic" }, result.Games.Select(x => x.Name));
        Assert.Equal(85, result.CompletenessPercent);
    }
}
