using HireGameDev.Domain.Entities;

namespace HireGameDev.Domain.Tests.Entities;

public class ShippedTitleTests
{
    private static readonly Guid ProfileId = Guid.NewGuid();

    [Fact]
    public void Stores_trimmed_values()
    {
        var title = new ShippedTitle(ProfileId, " Raji: An Ancient Epic ", "Nodding Heads Games", "Programmer", 2020);

        Assert.Equal("Raji: An Ancient Epic", title.GameName);
        Assert.Equal(2020, title.Year);
        Assert.False(title.IsVerified);
    }

    [Fact]
    public void Requires_a_profile()
    {
        Assert.Throws<ArgumentException>(() => new ShippedTitle(Guid.Empty, "Raji", "Studio", "Role", 2020));
    }

    [Theory]
    [InlineData("", "Studio", "Role")]
    [InlineData("Game", " ", "Role")]
    [InlineData("Game", "Studio", "")]
    public void Requires_game_studio_and_role(string game, string studio, string role)
    {
        Assert.Throws<ArgumentException>(() => new ShippedTitle(ProfileId, game, studio, role, 2020));
    }

    [Fact]
    public void Rejects_years_before_the_earliest()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new ShippedTitle(ProfileId, "Game", "Studio", "Role", ShippedTitle.EarliestYear - 1));
    }

    [Fact]
    public void Allows_next_year_but_nothing_later()
    {
        var nextYear = ShippedTitle.LatestYear();

        var title = new ShippedTitle(ProfileId, "Game", "Studio", "Role", nextYear);

        Assert.Equal(nextYear, title.Year);
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new ShippedTitle(ProfileId, "Game", "Studio", "Role", nextYear + 1));
    }
}
