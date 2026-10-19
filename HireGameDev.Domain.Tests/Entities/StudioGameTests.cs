using HireGameDev.Domain.Entities;

namespace HireGameDev.Domain.Tests.Entities;

public class StudioGameTests
{
    private static readonly Guid ProfileId = Guid.NewGuid();

    [Fact]
    public void Stores_trimmed_game_name_and_valid_optional_url()
    {
        var game = new StudioGame(ProfileId, " Raji: An Ancient Epic ", 2020, " https://example.com/store ");

        Assert.Equal("Raji: An Ancient Epic", game.Name);
        Assert.Equal("https://example.com/store", game.Url);
        Assert.Equal(2020, game.Year);
    }

    [Fact]
    public void Requires_a_studio_profile_and_a_name()
    {
        Assert.Throws<ArgumentException>(() => new StudioGame(Guid.Empty, "Game", 2020, null));
        Assert.Throws<ArgumentException>(() => new StudioGame(ProfileId, " ", 2020, null));
    }

    [Fact]
    public void Rejects_non_web_urls()
    {
        Assert.Throws<ArgumentException>(() => new StudioGame(ProfileId, "Game", 2020, "javascript:alert(1)"));
    }

    [Fact]
    public void Rejects_years_outside_the_supported_range()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new StudioGame(ProfileId, "Game", StudioGame.EarliestYear - 1, null));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new StudioGame(ProfileId, "Game", StudioGame.LatestYear() + 1, null));
    }
}
