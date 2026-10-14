using HireGameDev.Domain.Enums;
using static System.Net.Mime.MediaTypeNames;

namespace HireGameDev.Domain.Entities;

public class Job : BaseEntity
{
    private Job()
    {
    }

    public Job(
        Guid studioProfileId,
        string title,
        JobType type,
        SeniorityLevel seniorityLevel,
        decimal minimumAnnualSalary,
        decimal maximumAnnualSalary,
        string city)
    {
        StudioProfileId = studioProfileId;
        Title = title;
        Type = type;
        SeniorityLevel = seniorityLevel;
        MinimumAnnualSalary = minimumAnnualSalary;
        MaximumAnnualSalary = maximumAnnualSalary;
        City = city;
    }

    public Guid StudioProfileId { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public JobType Type { get; private set; }

    public SeniorityLevel SeniorityLevel { get; private set; }

    public decimal MinimumAnnualSalary { get; private set; }

    public decimal MaximumAnnualSalary { get; private set; }

    public string City { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public JobStatus Status { get; private set; }

    public DateTime? ExpiresAtUtc { get; private set; }

    public bool IsRemote { get; private set; }

    public ICollection<JobSkill> JobSkills { get; private set; }
        = new List<JobSkill>();

    public ICollection<Application> Applications { get; private set; }
        = new List<Application>();

    public StudioProfile StudioProfile { get; private set; } = null!;
}