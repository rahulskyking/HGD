using HireGameDev.Domain.Enums;

namespace HireGameDev.Domain.Entities;

public class CandidateProfile : BaseEntity
{
    private CandidateProfile()
    {
    }

    public CandidateProfile(Guid userId)
    {
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

    public decimal? CurrentAnnualSalary { get; private set; }

    public decimal ExpectedAnnualSalary { get; private set; }

    public NoticePeriod NoticePeriod { get; private set; }

    public bool OpenToRemote { get; private set; }

    public OpenToWorkStatus OpenToWorkStatus { get; private set; }

    public ICollection<CandidateSkill> CandidateSkills { get; private set; }
        = new List<CandidateSkill>();

    public ICollection<ShippedTitle> ShippedTitles { get; private set; }
        = new List<ShippedTitle>();
}