using HireGameDev.Domain.Entities;

namespace HireGameDev.Domain.Tests.Entities;

public class PortfolioLinkTests
{
    private static readonly Guid ProfileId = Guid.NewGuid();

    [Theory]
    [InlineData("https://www.artstation.com/ananya")]
    [InlineData("http://example.com")]
    [InlineData("  https://github.com/ananya  ")]
    public void Accepts_absolute_web_addresses(string url)
    {
        var link = new PortfolioLink(ProfileId, url, null);

        Assert.Equal(url.Trim(), link.Url);
        Assert.Null(link.Label);
    }

    [Theory]
    [InlineData("")]
    [InlineData("artstation.com/ananya")]
    [InlineData("ftp://example.com/file")]
    [InlineData("javascript:alert(1)")]
    [InlineData("mailto:dev@example.com")]
    public void Rejects_anything_that_is_not_http_or_https(string url)
    {
        Assert.False(PortfolioLink.IsValidUrl(url));
        Assert.Throws<ArgumentException>(() => new PortfolioLink(ProfileId, url, null));
    }

    [Fact]
    public void Rejects_urls_over_the_limit()
    {
        var url = "https://example.com/" + new string('a', PortfolioLink.UrlMaxLength);

        Assert.False(PortfolioLink.IsValidUrl(url));
    }

    [Fact]
    public void Label_is_trimmed_and_optional()
    {
        Assert.Equal("GitHub", new PortfolioLink(ProfileId, "https://github.com/a", "  GitHub ").Label);
        Assert.Null(new PortfolioLink(ProfileId, "https://github.com/a", "   ").Label);
        Assert.Throws<ArgumentException>(() =>
            new PortfolioLink(ProfileId, "https://github.com/a", new string('x', PortfolioLink.LabelMaxLength + 1)));
    }

    [Fact]
    public void Requires_a_profile()
    {
        Assert.Throws<ArgumentException>(() => new PortfolioLink(Guid.Empty, "https://github.com/a", null));
    }
}
