using HireGameDev.Domain;
using System.Text.RegularExpressions;
using HireGameDev.Domain.Enums;

namespace HireGameDev.Domain.Entities;

/// <summary>
/// A studio's company profile: one per studio account, created the first time the studio
/// saves it. Jobs are posted under it (Step 6) and candidates browse it (Step 7). Every rule
/// about a studio profile — lengths, the GSTIN format, how many engines, genres or games may
/// be listed, what "complete" means — lives here so all use cases enforce the same thing.
/// Verification (<see cref="IsVerified"/>) is granted by an admin later (Step 12); until
/// then every profile is unverified.
/// </summary>
public class StudioProfile : BaseEntity
{
    public const int NameMaxLength = 200;
    public const int CityMaxLength = 100;
    public const int StateMaxLength = 100;
    public const int AboutMaxLength = 600;
    public const int LogoUrlMaxLength = 500;
    public const int WebsiteUrlMaxLength = WebUrl.MaxLength;
    public const int GstNumberLength = 15;

    public const int MaximumEngines = 20;
    public const int MaximumGenres = 20;
    public const int MaximumCultureTags = 12;
    public const int MaximumGames = 20;

    // GSTIN: 2-digit state code, 10-character PAN, entity number, the letter Z, check character.
    private static readonly Regex GstNumberPattern =
        new("^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z][1-9A-Z]Z[0-9A-Z]$", RegexOptions.CultureInvariant);

    private readonly List<StudioEngine> _engines = [];
    private readonly List<StudioGenre> _genres = [];
    private readonly List<StudioCultureTag> _cultureTags = [];
    private readonly List<StudioGame> _games = [];

    private StudioProfile()
    {
    }

    public StudioProfile(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("A profile must belong to an account.", nameof(userId));
        }

        UserId = userId;
    }

    public Guid UserId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string? LogoUrl { get; private set; }

    public string? WebsiteUrl { get; private set; }

    public string City { get; private set; } = string.Empty;

    public string State { get; private set; } = string.Empty;

    /// <summary>Head-count band; stored as an integer in PostgreSQL.</summary>
    public StudioSize Size { get; private set; }

    /// <summary>Business type; stored as an integer in PostgreSQL.</summary>
    public StudioType Type { get; private set; }

    /// <summary>What the studio makes and what it is like to work there, in its own words.</summary>
    public string? About { get; private set; }

    /// <summary>Optional GSTIN, kept upper-case; shown to admins for verification, never to candidates.</summary>
    public string? GstNumber { get; private set; }

    public bool IsVerified { get; private set; }

    /// <summary>Engines the studio builds with, as rows referencing the Engines category.</summary>
    public IReadOnlyCollection<StudioEngine> Engines => _engines;

    /// <summary>Genres the studio makes, stored as enum values on their own join table.</summary>
    public IReadOnlyCollection<StudioGenre> Genres => _genres;

    public IReadOnlyCollection<StudioCultureTag> CultureTags => _cultureTags;

    /// <summary>Games the studio has released.</summary>
    public IReadOnlyCollection<StudioGame> Games => _games;

    public ICollection<Job> Jobs { get; private set; }
        = new List<Job>();

    public void UpdateBasics(
        string name,
        string? websiteUrl,
        string city,
        StudioSize size,
        StudioType type,
        string? about,
        string? gstNumber)
    {
        Name = Required(name, NameMaxLength, nameof(name));
        WebsiteUrl = WebUrl.Optional(websiteUrl, nameof(websiteUrl));

        var location = IndianCity.Find(city)
            ?? throw new ArgumentException("Choose a city from the list.", nameof(city));
        City = location.Name;
        State = location.State;

        Size = Defined(size, nameof(size));
        Type = Defined(type, nameof(type));
        About = Optional(about, AboutMaxLength, nameof(about));
        GstNumber = ValidGstNumber(gstNumber);

        Touch();
    }

    /// <summary>Makes the selected engines the studio's engines; kept rows are not replaced.</summary>
    public void ReplaceEngines(IReadOnlyCollection<Guid> skillIds)
    {
        ArgumentNullException.ThrowIfNull(skillIds);

        var wanted = skillIds.Distinct().ToList();

        if (wanted.Count > MaximumEngines)
        {
            throw new ArgumentException($"Pick at most {MaximumEngines} engines.", nameof(skillIds));
        }

        _engines.RemoveAll(x => !wanted.Contains(x.SkillId));

        foreach (var skillId in wanted.Where(id => _engines.All(x => x.SkillId != id)))
        {
            _engines.Add(new StudioEngine(Id, skillId));
        }

        Touch();
    }

    public void ReplaceGenres(IReadOnlyCollection<GameGenre> genres)
    {
        ArgumentNullException.ThrowIfNull(genres);

        var wanted = genres.Distinct().Select(x => Defined(x, nameof(genres))).ToList();

        if (wanted.Count > MaximumGenres)
        {
            throw new ArgumentException($"Pick at most {MaximumGenres} genres.", nameof(genres));
        }

        _genres.RemoveAll(x => !wanted.Contains(x.Genre));

        foreach (var genre in wanted.Where(genre => _genres.All(x => x.Genre != genre)))
        {
            _genres.Add(new StudioGenre(Id, genre));
        }

        Touch();
    }

    public void ReplaceCultureTags(IReadOnlyCollection<CultureTag> tags)
    {
        ArgumentNullException.ThrowIfNull(tags);

        var wanted = tags.Distinct().Select(x => Defined(x, nameof(tags))).ToList();

        if (wanted.Count > MaximumCultureTags)
        {
            throw new ArgumentException($"Pick at most {MaximumCultureTags} culture tags.", nameof(tags));
        }

        _cultureTags.RemoveAll(x => !wanted.Contains(x.Tag));

        foreach (var tag in wanted.Where(tag => _cultureTags.All(x => x.Tag != tag)))
        {
            _cultureTags.Add(new StudioCultureTag(Id, tag));
        }

        Touch();
    }

    /// <summary>Replaces the whole list; the edit form always posts the full list.</summary>
    public void ReplaceGames(IReadOnlyCollection<StudioGameDetails> games)
    {
        ArgumentNullException.ThrowIfNull(games);

        if (games.Count > MaximumGames)
        {
            throw new ArgumentException($"Add at most {MaximumGames} games.", nameof(games));
        }

        _games.Clear();

        foreach (var game in games)
        {
            _games.Add(new StudioGame(Id, game.Name, game.Year, game.Url));
        }

        Touch();
    }

    public void SetLogo(string logoUrl)
    {
        LogoUrl = Required(logoUrl, LogoUrlMaxLength, nameof(logoUrl));
        Touch();
    }

    public void RemoveLogo()
    {
        LogoUrl = null;
        Touch();
    }

    /// <summary>
    /// How much of the profile is filled in, 0–100. The basics are required to save, so a saved
    /// profile starts at 50; logo, About, website, engines/genres, games and culture tags make
    /// up the rest. Shown on the dashboard as a nudge (no hard gate).
    /// </summary>
    public int CompletenessPercent()
    {
        var percent = 50;

        if (!string.IsNullOrEmpty(LogoUrl))
        {
            percent += 15;
        }

        if (!string.IsNullOrEmpty(About))
        {
            percent += 10;
        }

        if (!string.IsNullOrEmpty(WebsiteUrl))
        {
            percent += 5;
        }

        if (_engines.Count > 0 || _genres.Count > 0)
        {
            percent += 10;
        }

        if (_games.Count > 0)
        {
            percent += 5;
        }

        if (_cultureTags.Count > 0)
        {
            percent += 5;
        }

        return percent;
    }

    public static bool IsValidGstNumber(string? gstNumber)
    {
        var normalized = gstNumber?.Trim().ToUpperInvariant();

        return normalized is not null
            && GstNumberPattern.IsMatch(normalized)
            && normalized[..2] != "00";
    }

    private static string? ValidGstNumber(string? gstNumber)
    {
        if (string.IsNullOrWhiteSpace(gstNumber))
        {
            return null;
        }

        if (!IsValidGstNumber(gstNumber))
        {
            throw new ArgumentException("Enter a valid 15-character GSTIN, e.g. 29ABCDE1234F1Z5.", nameof(gstNumber));
        }

        return gstNumber.Trim().ToUpperInvariant();
    }

    private void Touch() => UpdatedAtUtc = DateTime.UtcNow;

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

    private static string? Optional(string? value, int maxLength, string paramName)
    {
        var trimmed = value?.Trim();

        if (string.IsNullOrEmpty(trimmed))
        {
            return null;
        }

        if (trimmed.Length > maxLength)
        {
            throw new ArgumentException($"Must be {maxLength} characters or fewer.", paramName);
        }

        return trimmed;
    }

    private static TEnum Defined<TEnum>(TEnum value, string paramName)
        where TEnum : struct, Enum
    {
        if (!Enum.IsDefined(value))
        {
            throw new ArgumentOutOfRangeException(paramName, $"{value} is not a valid {typeof(TEnum).Name}.");
        }

        return value;
    }
}
