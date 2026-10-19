using HireGameDev.Domain;
using HireGameDev.Domain.Entities;
using HireGameDev.Domain.Enums;

namespace HireGameDev.Domain.Tests.Entities;

public class StudioProfileTests
{
    private static readonly Guid UserId = Guid.NewGuid();

    private static StudioProfile Profile() => new(UserId);

    private static void Update(
        StudioProfile profile,
        string name = "Nodding Heads Games",
        string? website = null,
        string city = "Pune",
        StudioSize size = StudioSize.From11To50,
        StudioType type = StudioType.GameStudio,
        string? about = null,
        string? gstNumber = null) =>
        profile.UpdateBasics(name, website, city, size, type, about, gstNumber);

    [Fact]
    public void Requires_a_studio_account()
    {
        Assert.Throws<ArgumentException>(() => new StudioProfile(Guid.Empty));
    }

    [Fact]
    public void Stores_trimmed_details_and_derives_the_state_from_the_city()
    {
        var profile = Profile();

        Update(profile, name: "  Nodding Heads Games  ", city: " pune ",
            website: " https://noddingheadsgames.com ", gstNumber: "29abcde1234f1z5");

        Assert.Equal("Nodding Heads Games", profile.Name);
        Assert.Equal("Pune", profile.City);
        Assert.Equal("Maharashtra", profile.State);
        Assert.Equal("https://noddingheadsgames.com", profile.WebsiteUrl);
        Assert.Equal("29ABCDE1234F1Z5", profile.GstNumber);
        Assert.Equal(StudioSize.From11To50, profile.Size);
        Assert.Equal(StudioType.GameStudio, profile.Type);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Requires_a_studio_name(string name)
    {
        Assert.Throws<ArgumentException>(() => Update(Profile(), name: name));
    }

    [Fact]
    public void Requires_a_city_from_the_curated_list()
    {
        Assert.Throws<ArgumentException>(() => Update(Profile(), city: "Small town"));
    }

    [Fact]
    public void Rejects_non_web_urls()
    {
        Assert.Throws<ArgumentException>(() => Update(Profile(), website: "javascript:alert(1)"));
    }

    [Theory]
    [InlineData("29ABCDE1234F1Z")]
    [InlineData("00ABCDE1234F1Z5")]
    [InlineData("29ABC1234F1Z5")]
    public void Rejects_malformed_gstin(string gstNumber)
    {
        Assert.Throws<ArgumentException>(() => Update(Profile(), gstNumber: gstNumber));
    }

    [Fact]
    public void Gstin_is_optional_and_case_is_normalized()
    {
        Update(Profile(), gstNumber: null);
        Assert.True(StudioProfile.IsValidGstNumber("29abcde1234f1z5"));
        Assert.False(StudioProfile.IsValidGstNumber("not-a-gstin"));
    }

    [Fact]
    public void Rejects_undefined_studio_enums()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Update(Profile(), size: (StudioSize)99));
        Assert.Throws<ArgumentOutOfRangeException>(() => Update(Profile(), type: (StudioType)99));
    }

    [Fact]
    public void Replaces_engines_genres_culture_tags_and_games_as_the_aggregate()
    {
        var profile = Profile();
        Update(profile, website: "https://example.com", about: "We make thoughtful games.");

        var unity = Guid.NewGuid();
        var unreal = Guid.NewGuid();
        profile.ReplaceEngines([unity, unreal, unity]);
        profile.ReplaceGenres([GameGenre.Puzzle, GameGenre.Match3, GameGenre.Puzzle]);
        profile.ReplaceCultureTags([CultureTag.NoCrunch, CultureTag.FlexibleHours, CultureTag.NoCrunch]);
        profile.ReplaceGames(
        [
            new StudioGameDetails("Raji: An Ancient Epic", 2020, "https://store.steampowered.com/app/"),
        ]);

        Assert.Equal(new[] { unity, unreal }.OrderBy(x => x), profile.Engines.Select(x => x.SkillId).OrderBy(x => x));
        Assert.Equal(new[] { GameGenre.Puzzle, GameGenre.Match3 }, profile.Genres.Select(x => x.Genre).OrderBy(x => x));
        Assert.Equal(new[] { CultureTag.NoCrunch, CultureTag.FlexibleHours }, profile.CultureTags.Select(x => x.Tag).OrderBy(x => x));
        Assert.Single(profile.Games);
        Assert.Equal(85, profile.CompletenessPercent());

        profile.SetLogo("/media/logos/mark.png");
        Assert.Equal(100, profile.CompletenessPercent());

        profile.ReplaceGames([]);
        Assert.Empty(profile.Games);
    }

    [Fact]
    public void Rejects_undefined_genres_and_culture_tags()
    {
        var profile = Profile();

        Assert.Throws<ArgumentOutOfRangeException>(() => profile.ReplaceGenres([(GameGenre)99]));
        Assert.Throws<ArgumentOutOfRangeException>(() => profile.ReplaceCultureTags([(CultureTag)99]));
    }

    [Fact]
    public void Caps_engine_genre_and_game_lists()
    {
        var profile = Profile();

        Assert.Throws<ArgumentException>(() => profile.ReplaceEngines(
            Enumerable.Range(0, StudioProfile.MaximumEngines + 1).Select(_ => Guid.NewGuid()).ToList()));
        Assert.Throws<ArgumentException>(() => profile.ReplaceGenres(
            Enum.GetValues<GameGenre>().Take(StudioProfile.MaximumGenres + 1).ToList()));
        Assert.Throws<ArgumentException>(() => profile.ReplaceGames(
            Enumerable.Range(0, StudioProfile.MaximumGames + 1)
                .Select(i => new StudioGameDetails($"Game {i}", 2020, null)).ToList()));
    }

    [Fact]
    public void Rejects_about_over_the_limit()
    {
        Assert.Throws<ArgumentException>(() => Update(
            Profile(), about: new string('a', StudioProfile.AboutMaxLength + 1)));
    }
}
