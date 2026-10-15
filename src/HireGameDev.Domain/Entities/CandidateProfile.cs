using HireGameDev.Domain.Enums;

namespace HireGameDev.Domain.Entities;

/// <summary>
/// A game developer's profile: one per candidate account, created the first time the
/// candidate saves it. Every rule about a profile (lengths, ranges, the minimum number of
/// skills, what "complete" means) lives here so all use cases enforce the same thing.
/// Salaries are stored in rupees per year; presenting them as LPA is a UI concern.
/// </summary>
public class CandidateProfile : BaseEntity
{
    public const int FullNameMaxLength = 150;
    public const int PhoneMaxLength = 20;
    public const int CityMaxLength = 100;
    public const int JobTitleMaxLength = 150;
    public const int AboutMaxLength = 600;
    public const int PhotoUrlMaxLength = 500;

    public const int MinimumSkillCount = 3;
    public const int MaximumSkillCount = 30;
    public const int MaximumShippedTitles = 20;
    public const int MaximumPortfolioLinks = 6;
    public const int MaximumYearsOfExperience = 50;

    private readonly List<CandidateSkill> _candidateSkills = [];
    private readonly List<ShippedTitle> _shippedTitles = [];
    private readonly List<PortfolioLink> _portfolioLinks = [];

    private CandidateProfile()
    {
    }

    public CandidateProfile(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("A profile must belong to an account.", nameof(userId));
        }

        UserId = userId;
    }

    public Guid UserId { get; private set; }

    public string FullName { get; private set; } = string.Empty;

    public string? PhotoUrl { get; private set; }

    public string? Phone { get; private set; }

    public string City { get; private set; } = string.Empty;

    public string JobTitle { get; private set; } = string.Empty;

    public SeniorityLevel SeniorityLevel { get; private set; }

    public decimal YearsOfExperience { get; private set; }

    /// <summary>Rupees per year; optional because freshers and people between jobs have none.</summary>
    public decimal? CurrentAnnualSalary { get; private set; }

    /// <summary>Rupees per year. Required: it drives salary matching.</summary>
    public decimal ExpectedAnnualSalary { get; private set; }

    public NoticePeriod NoticePeriod { get; private set; }

    public bool OpenToRemote { get; private set; }

    public OpenToWorkStatus OpenToWorkStatus { get; private set; }

    /// <summary>A short introduction in the candidate's own words.</summary>
    public string? About { get; private set; }

    public IReadOnlyCollection<CandidateSkill> CandidateSkills => _candidateSkills;

    public IReadOnlyCollection<ShippedTitle> ShippedTitles => _shippedTitles;

    public IReadOnlyCollection<PortfolioLink> PortfolioLinks => _portfolioLinks;

    public void UpdateBasics(
        string fullName,
        string? phone,
        string city,
        string jobTitle,
        SeniorityLevel seniorityLevel,
        decimal yearsOfExperience,
        decimal? currentAnnualSalary,
        decimal expectedAnnualSalary,
        NoticePeriod noticePeriod,
        bool openToRemote,
        OpenToWorkStatus openToWorkStatus,
        string? about)
    {
        FullName = Required(fullName, FullNameMaxLength, nameof(fullName));
        Phone = Optional(phone, PhoneMaxLength, nameof(phone));
        City = Required(city, CityMaxLength, nameof(city));
        JobTitle = Required(jobTitle, JobTitleMaxLength, nameof(jobTitle));
        SeniorityLevel = Defined(seniorityLevel, nameof(seniorityLevel));
        NoticePeriod = Defined(noticePeriod, nameof(noticePeriod));
        OpenToWorkStatus = Defined(openToWorkStatus, nameof(openToWorkStatus));

        if (yearsOfExperience is < 0 or > MaximumYearsOfExperience)
        {
            throw new ArgumentOutOfRangeException(
                nameof(yearsOfExperience),
                $"Years of experience must be between 0 and {MaximumYearsOfExperience}.");
        }

        if (currentAnnualSalary is < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(currentAnnualSalary),
                "Current salary cannot be negative.");
        }

        if (expectedAnnualSalary <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(expectedAnnualSalary),
                "Expected salary must be greater than zero.");
        }

        YearsOfExperience = yearsOfExperience;
        CurrentAnnualSalary = currentAnnualSalary;
        ExpectedAnnualSalary = expectedAnnualSalary;
        OpenToRemote = openToRemote;
        About = Optional(about, AboutMaxLength, nameof(about));

        Touch();
    }

    /// <summary>
    /// Makes the given skills the profile's skills. Rows for skills that stay are kept;
    /// the rest are removed and new ones added.
    /// </summary>
    public void ReplaceSkills(IReadOnlyCollection<Guid> skillIds)
    {
        ArgumentNullException.ThrowIfNull(skillIds);

        var wanted = skillIds.Distinct().ToList();

        if (wanted.Count < MinimumSkillCount)
        {
            throw new ArgumentException(
                $"Pick at least {MinimumSkillCount} skills.", nameof(skillIds));
        }

        if (wanted.Count > MaximumSkillCount)
        {
            throw new ArgumentException(
                $"Pick at most {MaximumSkillCount} skills.", nameof(skillIds));
        }

        _candidateSkills.RemoveAll(x => !wanted.Contains(x.SkillId));

        foreach (var skillId in wanted.Where(id => _candidateSkills.All(x => x.SkillId != id)))
        {
            _candidateSkills.Add(new CandidateSkill(Id, skillId));
        }

        Touch();
    }

    /// <summary>
    /// Replaces the whole list; the edit form always posts the full list. Verification flags
    /// are not carried over because titles cannot be verified yet (see ADR-017).
    /// </summary>
    public void ReplaceShippedTitles(IReadOnlyCollection<ShippedTitleDetails> titles)
    {
        ArgumentNullException.ThrowIfNull(titles);

        if (titles.Count > MaximumShippedTitles)
        {
            throw new ArgumentException(
                $"Add at most {MaximumShippedTitles} shipped titles.", nameof(titles));
        }

        _shippedTitles.Clear();

        foreach (var title in titles)
        {
            _shippedTitles.Add(new ShippedTitle(Id, title.GameName, title.StudioName, title.Role, title.Year));
        }

        Touch();
    }

    public void ReplacePortfolioLinks(IReadOnlyCollection<PortfolioLinkDetails> links)
    {
        ArgumentNullException.ThrowIfNull(links);

        if (links.Count > MaximumPortfolioLinks)
        {
            throw new ArgumentException(
                $"Add at most {MaximumPortfolioLinks} portfolio links.", nameof(links));
        }

        _portfolioLinks.Clear();

        foreach (var link in links)
        {
            _portfolioLinks.Add(new PortfolioLink(Id, link.Url, link.Label));
        }

        Touch();
    }

    public void SetPhoto(string photoUrl)
    {
        PhotoUrl = Required(photoUrl, PhotoUrlMaxLength, nameof(photoUrl));
        Touch();
    }

    public void RemovePhoto()
    {
        PhotoUrl = null;
        Touch();
    }

    /// <summary>
    /// How much of the profile is filled in, 0–100. Basics and skills are required to save,
    /// so a saved profile starts at 50; photo, About, a shipped title and a portfolio link
    /// make up the rest. Shown on the dashboard to nudge candidates (no hard gate).
    /// </summary>
    public int CompletenessPercent()
    {
        var percent = 50;

        if (!string.IsNullOrEmpty(PhotoUrl))
        {
            percent += 15;
        }

        if (!string.IsNullOrEmpty(About))
        {
            percent += 10;
        }

        if (_shippedTitles.Count > 0)
        {
            percent += 15;
        }

        if (_portfolioLinks.Count > 0)
        {
            percent += 10;
        }

        return percent;
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
