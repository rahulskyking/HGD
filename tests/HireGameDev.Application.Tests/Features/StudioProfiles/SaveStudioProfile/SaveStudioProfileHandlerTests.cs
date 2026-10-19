using HireGameDev.Application.Features.StudioProfiles.SaveStudioProfile;
using HireGameDev.Application.Tests.Persistence;
using HireGameDev.Domain.Entities;
using HireGameDev.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HireGameDev.Application.Tests.Features.StudioProfiles.SaveStudioProfile;

public class SaveStudioProfileHandlerTests : IDisposable
{
    private readonly SqliteDatabase _database = new();

    public void Dispose() => _database.Dispose();

    [Fact]
    public async Task Creates_a_studio_profile_with_its_engines_genres_culture_and_games()
    {
        var userId = await _database.AddUserAsync();
        var engines = await _database.AddSkillsAsync("Unity", "Unreal Engine 5");

        var result = await Handle(Command(userId, engines.Select(x => x.Id).ToList()));

        Assert.True(result.Succeeded);

        await using var context = _database.CreateContext();
        var profile = await context.StudioProfiles
            .Include(x => x.Engines)
            .Include(x => x.Genres)
            .Include(x => x.CultureTags)
            .Include(x => x.Games)
            .SingleAsync(x => x.UserId == userId);

        Assert.Equal("Nodding Heads Games", profile.Name);
        Assert.Equal("Pune", profile.City);
        Assert.Equal("Maharashtra", profile.State);
        Assert.Equal(StudioSize.From11To50, profile.Size);
        Assert.Equal(StudioType.GameStudio, profile.Type);
        Assert.Equal("29ABCDE1234F1Z5", profile.GstNumber);
        Assert.Equal(2, profile.Engines.Count);
        Assert.Equal(new[] { GameGenre.Puzzle, GameGenre.Rpg }, profile.Genres.Select(x => x.Genre).OrderBy(x => x));
        Assert.Equal(new[] { CultureTag.NoCrunch, CultureTag.FiveDayWeek }, profile.CultureTags.Select(x => x.Tag).OrderBy(x => x));
        Assert.Single(profile.Games);
    }

    [Fact]
    public async Task Second_save_updates_the_same_profile_and_replaces_its_lists()
    {
        var userId = await _database.AddUserAsync();
        var engines = await _database.AddSkillsAsync("Unity", "Unreal Engine 5", "Godot");
        var ids = engines.Select(x => x.Id).ToList();

        await Handle(Command(userId, ids.Take(2).ToList()));
        var update = Command(userId, [ids[2]]) with
        {
            Name = "Nodding Heads",
            City = "Kolkata",
            Genres = [GameGenre.NarrativeAdventure],
            CultureTags = [CultureTag.RemoteFriendly],
            Games = [],
        };

        var result = await Handle(update);

        Assert.True(result.Succeeded);
        await using var context = _database.CreateContext();
        Assert.Equal(1, await context.StudioProfiles.CountAsync());

        var profile = await context.StudioProfiles
            .Include(x => x.Engines)
            .Include(x => x.Genres)
            .Include(x => x.CultureTags)
            .Include(x => x.Games)
            .SingleAsync();

        Assert.Equal("Nodding Heads", profile.Name);
        Assert.Equal("West Bengal", profile.State);
        Assert.Equal(new[] { ids[2] }, profile.Engines.Select(x => x.SkillId));
        Assert.Equal(new[] { GameGenre.NarrativeAdventure }, profile.Genres.Select(x => x.Genre));
        Assert.Equal(new[] { CultureTag.RemoteFriendly }, profile.CultureTags.Select(x => x.Tag));
        Assert.Empty(profile.Games);
        Assert.Equal(0, await context.StudioGames.CountAsync());
    }

    [Fact]
    public async Task Fails_when_a_selected_engine_is_not_from_the_engines_catalogue()
    {
        var userId = await _database.AddUserAsync();
        var language = new Skill("C#", SkillCategory.Languages);
        await using (var setup = _database.CreateContext())
        {
            setup.Skills.Add(language);
            await setup.SaveChangesAsync();
        }

        var result = await Handle(Command(userId, [language.Id]));

        Assert.False(result.Succeeded);
        await using var context = _database.CreateContext();
        Assert.Equal(0, await context.StudioProfiles.CountAsync());
    }

    [Fact]
    public async Task Returns_a_generic_failure_when_domain_rules_are_broken()
    {
        var userId = await _database.AddUserAsync();

        var result = await Handle(Command(userId, []) with { City = "Not a city" });

        Assert.False(result.Succeeded);
        Assert.Contains("not valid", result.Errors.Single().ToLowerInvariant());
        await using var context = _database.CreateContext();
        Assert.Equal(0, await context.StudioProfiles.CountAsync());
    }

    private async Task<OperationResult> Handle(SaveStudioProfileCommand command)
    {
        await using var context = _database.CreateContext();
        return await new SaveStudioProfileHandler(context).HandleAsync(command);
    }

    private static SaveStudioProfileCommand Command(Guid userId, IReadOnlyCollection<Guid> engineIds) => new(
        UserId: userId,
        Name: "Nodding Heads Games",
        WebsiteUrl: "https://noddingheadsgames.com",
        City: "Pune",
        Size: StudioSize.From11To50,
        Type: StudioType.GameStudio,
        About: "We build story-rich games.",
        GstNumber: "29ABCDE1234F1Z5",
        EngineIds: engineIds,
        Genres: [GameGenre.Puzzle, GameGenre.Rpg],
        CultureTags: [CultureTag.NoCrunch, CultureTag.FiveDayWeek],
        Games: [new StudioGameDetails("Raji: An Ancient Epic", 2020, "https://store.steampowered.com/app/")]);
}
