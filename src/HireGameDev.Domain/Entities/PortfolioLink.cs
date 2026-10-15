namespace HireGameDev.Domain.Entities;

/// <summary>
/// A link to work outside the platform: ArtStation, GitHub, Behance, YouTube, itch.io, a personal
/// site. Only absolute http(s) URLs are accepted so a link can never run script when rendered.
/// </summary>
public class PortfolioLink : BaseEntity
{
    public const int UrlMaxLength = 500;
    public const int LabelMaxLength = 60;

    private PortfolioLink()
    {
    }

    public PortfolioLink(Guid candidateProfileId, string url, string? label)
    {
        if (candidateProfileId == Guid.Empty)
        {
            throw new ArgumentException("A portfolio link must belong to a profile.", nameof(candidateProfileId));
        }

        CandidateProfileId = candidateProfileId;
        Url = ValidUrl(url);

        var trimmedLabel = label?.Trim();

        if (trimmedLabel is { Length: > LabelMaxLength })
        {
            throw new ArgumentException($"Must be {LabelMaxLength} characters or fewer.", nameof(label));
        }

        Label = string.IsNullOrEmpty(trimmedLabel) ? null : trimmedLabel;
    }

    public Guid CandidateProfileId { get; private set; }

    public string Url { get; private set; } = string.Empty;

    /// <summary>Optional caption, e.g. "Play Store games"; the host name is shown when absent.</summary>
    public string? Label { get; private set; }

    public CandidateProfile CandidateProfile { get; private set; } = null!;

    public static bool IsValidUrl(string? url) =>
        url is not null
        && url.Length <= UrlMaxLength
        && Uri.TryCreate(url.Trim(), UriKind.Absolute, out var uri)
        && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
        && !string.IsNullOrEmpty(uri.Host);

    private static string ValidUrl(string? url)
    {
        if (!IsValidUrl(url))
        {
            throw new ArgumentException("Enter a full web address starting with http:// or https://.", nameof(url));
        }

        return url!.Trim();
    }
}
