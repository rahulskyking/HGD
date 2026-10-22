using HireGameDev.Domain.Enums;

namespace HireGameDev.Domain.Entities;

public class Job : BaseEntity
{
    public const int TitleMaxLength = 200;
    public const int CityMaxLength = 100;

    private Job()
    {
    }

    public Job(Guid studioProfileId)
    {
        if (studioProfileId == Guid.Empty)
        {
            throw new ArgumentException("A studio profile is required.", nameof(studioProfileId));
        }

        StudioProfileId = studioProfileId;
        Status = JobStatus.Draft;
    }

    public Guid StudioProfileId { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public JobCategory Category { get; private set; } = JobCategory.Other;

    public JobType Type { get; private set; }

    public SeniorityLevel SeniorityLevel { get; private set; }

    public decimal MinimumAnnualSalary { get; private set; }

    public decimal MaximumAnnualSalary { get; private set; }

    public string City { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public JobStatus Status { get; private set; }

    public DateTime? ExpiresAtUtc { get; private set; }

    public LocationMode LocationMode { get; private set; }

    public ICollection<JobSkill> JobSkills { get; private set; }
        = new List<JobSkill>();

    public ICollection<Application> Applications { get; private set; }
        = new List<Application>();

    public StudioProfile StudioProfile { get; private set; } = null!;

    public void UpdateDetails(
        string title,
        JobCategory category,
        JobType type,
        SeniorityLevel seniorityLevel,
        decimal minimumAnnualSalary,
        decimal maximumAnnualSalary,
        LocationMode locationMode,
        string city,
        string description)
    {
        var normalizedTitle = Required(title, TitleMaxLength, nameof(title));
        var normalizedCity = Required(city, CityMaxLength, nameof(city));
        var normalizedDescription = Required(description, int.MaxValue, nameof(description));

        if (!Enum.IsDefined(category))
        {
            throw new ArgumentOutOfRangeException(nameof(category));
        }

        if (!Enum.IsDefined(type))
        {
            throw new ArgumentOutOfRangeException(nameof(type));
        }

        if (!Enum.IsDefined(seniorityLevel))
        {
            throw new ArgumentOutOfRangeException(nameof(seniorityLevel));
        }

        if (!Enum.IsDefined(locationMode))
        {
            throw new ArgumentOutOfRangeException(nameof(locationMode));
        }

        if (minimumAnnualSalary <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(minimumAnnualSalary),
                "The minimum annual salary must be greater than zero.");
        }

        if (maximumAnnualSalary < minimumAnnualSalary)
        {
            throw new ArgumentException(
                "The maximum annual salary must be at least the minimum annual salary.",
                nameof(maximumAnnualSalary));
        }

        Title = normalizedTitle;
        Category = category;
        Type = type;
        SeniorityLevel = seniorityLevel;
        MinimumAnnualSalary = minimumAnnualSalary;
        MaximumAnnualSalary = maximumAnnualSalary;
        LocationMode = locationMode;
        City = normalizedCity;
        Description = normalizedDescription;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void ReplaceSkills(
        IReadOnlyCollection<Guid> requiredSkillIds,
        IReadOnlyCollection<Guid> niceToHaveSkillIds)
    {
        var requiredIds = ValidateSkillIds(requiredSkillIds, nameof(requiredSkillIds));
        var niceToHaveIds = ValidateSkillIds(niceToHaveSkillIds, nameof(niceToHaveSkillIds));

        if (requiredIds.Overlaps(niceToHaveIds))
        {
            throw new ArgumentException("A skill cannot be both required and nice-to-have.");
        }

        JobSkills.Clear();

        foreach (var skillId in requiredIds)
        {
            JobSkills.Add(new JobSkill(Id, skillId, isRequired: true));
        }

        foreach (var skillId in niceToHaveIds)
        {
            JobSkills.Add(new JobSkill(Id, skillId, isRequired: false));
        }

        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Publish(DateTime expiresAtUtc)
    {
        if (Status != JobStatus.Draft)
        {
            throw new InvalidOperationException("Only a draft job can be published.");
        }

        EnsureReadyToPublish();
        EnsureUtc(expiresAtUtc, nameof(expiresAtUtc));

        if (expiresAtUtc <= DateTime.UtcNow)
        {
            throw new ArgumentOutOfRangeException(
                nameof(expiresAtUtc),
                "Choose an expiry date in the future.");
        }

        ExpiresAtUtc = expiresAtUtc;
        Status = JobStatus.Published;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Pause()
    {
        if (ExpireIfDue(DateTime.UtcNow))
        {
            throw new InvalidOperationException("This job has expired and can no longer be paused.");
        }

        if (Status != JobStatus.Published)
        {
            throw new InvalidOperationException("Only a published job can be paused.");
        }

        Status = JobStatus.Paused;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Resume()
    {
        if (ExpireIfDue(DateTime.UtcNow))
        {
            throw new InvalidOperationException("This job has expired and cannot be resumed.");
        }

        if (Status != JobStatus.Paused)
        {
            throw new InvalidOperationException("Only a paused job can be resumed.");
        }

        if (ExpiresAtUtc is null || ExpiresAtUtc <= DateTime.UtcNow)
        {
            throw new InvalidOperationException("This job has expired and cannot be resumed.");
        }

        Status = JobStatus.Published;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Close()
    {
        if (ExpireIfDue(DateTime.UtcNow))
        {
            throw new InvalidOperationException("This job has expired and can no longer be closed.");
        }

        if (Status is not (JobStatus.Draft or JobStatus.Published or JobStatus.Paused))
        {
            throw new InvalidOperationException("This job can no longer be closed.");
        }

        Status = JobStatus.Closed;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>Used by a background expiry process or an owner command when a job is due.</summary>
    public bool ExpireIfDue(DateTime nowUtc)
    {
        EnsureUtc(nowUtc, nameof(nowUtc));

        if (Status is not (JobStatus.Published or JobStatus.Paused)
            || ExpiresAtUtc is null
            || ExpiresAtUtc > nowUtc)
        {
            return false;
        }

        Status = JobStatus.Expired;
        UpdatedAtUtc = nowUtc;
        return true;
    }

    /// <summary>Shows expiry immediately even before a scheduled process persists the status.</summary>
    public JobStatus EffectiveStatusAt(DateTime nowUtc) =>
        (Status is JobStatus.Published or JobStatus.Paused)
            && ExpiresAtUtc is not null
            && ExpiresAtUtc <= nowUtc
                ? JobStatus.Expired
                : Status;

    private void EnsureReadyToPublish()
    {
        if (string.IsNullOrWhiteSpace(Title)
            || !Enum.IsDefined(Category)
            || !Enum.IsDefined(Type)
            || !Enum.IsDefined(SeniorityLevel)
            || MinimumAnnualSalary <= 0
            || MaximumAnnualSalary < MinimumAnnualSalary
            || !Enum.IsDefined(LocationMode)
            || string.IsNullOrWhiteSpace(City)
            || string.IsNullOrWhiteSpace(Description))
        {
            throw new InvalidOperationException("Complete all required job details before publishing.");
        }
    }

    private static HashSet<Guid> ValidateSkillIds(
        IReadOnlyCollection<Guid> skillIds,
        string parameterName)
    {
        ArgumentNullException.ThrowIfNull(skillIds);

        var values = skillIds.ToHashSet();

        if (values.Count != skillIds.Count || values.Contains(Guid.Empty))
        {
            throw new ArgumentException("Skill selections must be unique and valid.", parameterName);
        }

        return values;
    }

    private static string Required(string? value, int maxLength, string parameterName)
    {
        var normalized = value?.Trim();

        if (string.IsNullOrWhiteSpace(normalized))
        {
            throw new ArgumentException("A value is required.", parameterName);
        }

        if (normalized.Length > maxLength)
        {
            throw new ArgumentException($"Must be {maxLength} characters or fewer.", parameterName);
        }

        return normalized;
    }

    private static void EnsureUtc(DateTime value, string parameterName)
    {
        if (value.Kind != DateTimeKind.Utc)
        {
            throw new ArgumentException("The date must be in UTC.", parameterName);
        }
    }
}
