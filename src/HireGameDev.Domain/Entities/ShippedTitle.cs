namespace HireGameDev.Domain.Entities;

/// <summary>
/// A game the candidate worked on — the platform's primary credibility signal.
/// Verification (<see cref="IsVerified"/>) is a later feature; until then every title is self-declared.
/// </summary>
public class ShippedTitle : BaseEntity
{
    public const int GameNameMaxLength = 200;
    public const int StudioNameMaxLength = 200;
    public const int RoleMaxLength = 150;
    public const int EarliestYear = 1970;

    private ShippedTitle()
    {
    }

    public ShippedTitle(
        Guid candidateProfileId,
        string gameName,
        string studioName,
        string role,
        int year)
    {
        if (candidateProfileId == Guid.Empty)
        {
            throw new ArgumentException("A shipped title must belong to a profile.", nameof(candidateProfileId));
        }

        if (year < EarliestYear || year > LatestYear())
        {
            throw new ArgumentOutOfRangeException(
                nameof(year), $"Year must be between {EarliestYear} and {LatestYear()}.");
        }

        CandidateProfileId = candidateProfileId;
        GameName = Required(gameName, GameNameMaxLength, nameof(gameName));
        StudioName = Required(studioName, StudioNameMaxLength, nameof(studioName));
        Role = Required(role, RoleMaxLength, nameof(role));
        Year = year;
    }

    public Guid CandidateProfileId { get; private set; }

    public string GameName { get; private set; } = string.Empty;

    public string StudioName { get; private set; } = string.Empty;

    public string Role { get; private set; } = string.Empty;

    public int Year { get; private set; }

    public bool IsVerified { get; private set; }

    public CandidateProfile CandidateProfile { get; private set; } = null!;

    /// <summary>Titles announced for next year are allowed; anything further out is a typo.</summary>
    public static int LatestYear() => DateTime.UtcNow.Year + 1;

    private static string Required(string? value, int maxLength, string paramName)
    {
        var trimmed = value?.Trim();

        if (string.IsNullOrEmpty(trimmed))
        {
            throw new ArgumentException("A value is required.", paramName);
        }

        if (trimmed.Length > maxLength)
        {
            throw new ArgumentException($"Must be {maxLength} characters or fewer.", paramName);
        }

        return trimmed;
    }
}
