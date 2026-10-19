using HireGameDev.Domain;

namespace HireGameDev.Domain.Entities;

/// <summary>
/// A game the studio has released — its credibility signal, like a candidate's shipped titles.
/// The optional link points at a store page or trailer and follows the <see cref="WebUrl"/> rule.
/// </summary>
public class StudioGame : BaseEntity
{
    public const int NameMaxLength = 200;
    public const int UrlMaxLength = WebUrl.MaxLength;
    public const int EarliestYear = 1970;

    private StudioGame()
    {
    }

    public StudioGame(Guid studioProfileId, string name, int year, string? url)
    {
        if (studioProfileId == Guid.Empty)
        {
            throw new ArgumentException("A game must belong to a profile.", nameof(studioProfileId));
        }

        if (year < EarliestYear || year > LatestYear())
        {
            throw new ArgumentOutOfRangeException(
                nameof(year), $"Year must be between {EarliestYear} and {LatestYear()}.");
        }

        var trimmedName = name?.Trim();

        if (string.IsNullOrEmpty(trimmedName))
        {
            throw new ArgumentException("A value is required.", nameof(name));
        }

        if (trimmedName.Length > NameMaxLength)
        {
            throw new ArgumentException($"Must be {NameMaxLength} characters or fewer.", nameof(name));
        }

        StudioProfileId = studioProfileId;
        Name = trimmedName;
        Year = year;
        Url = WebUrl.Optional(url, nameof(url));
    }

    public Guid StudioProfileId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public int Year { get; private set; }

    public string? Url { get; private set; }

    public StudioProfile StudioProfile { get; private set; } = null!;

    /// <summary>Games announced for next year are allowed; anything further out is a typo.</summary>
    public static int LatestYear() => DateTime.UtcNow.Year + 1;
}
