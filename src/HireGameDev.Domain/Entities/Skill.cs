namespace HireGameDev.Domain.Entities;

/// <summary>
/// One entry of the skills master list (engines, languages, tools, genres, platforms, roles).
/// Candidates and jobs pick from this list; free-text skills are not allowed.
/// </summary>
public class Skill : BaseEntity
{
    public const int NameMaxLength = 100;
    public const int CategoryMaxLength = 100;

    private Skill()
    {
    }

    public Skill(string name, string category)
    {
        Name = Required(name, NameMaxLength, nameof(name));
        Category = Required(category, CategoryMaxLength, nameof(category));
    }

    public string Name { get; private set; } = string.Empty;

    public string Category { get; private set; } = string.Empty;

    public ICollection<CandidateSkill> CandidateSkills { get; private set; }
        = new List<CandidateSkill>();

    public ICollection<JobSkill> JobSkills { get; private set; }
        = new List<JobSkill>();

    /// <summary>Used when the catalogue regroups a skill; the name (and so every reference) stays.</summary>
    public void MoveToCategory(string category)
    {
        Category = Required(category, CategoryMaxLength, nameof(category));
        UpdatedAtUtc = DateTime.UtcNow;
    }

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
