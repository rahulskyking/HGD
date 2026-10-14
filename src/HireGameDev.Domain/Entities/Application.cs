using HireGameDev.Domain.Enums;

namespace HireGameDev.Domain.Entities;

public class Application : BaseEntity
{
    private Application()
    {
    }

    public Application(
        Guid candidateProfileId,
        Guid jobId)
    {
        CandidateProfileId = candidateProfileId;
        JobId = jobId;
        Status = ApplicationStatus.Applied;
        AppliedAtUtc = DateTime.UtcNow;
        LastActionAtUtc = AppliedAtUtc;
    }

    public Guid CandidateProfileId { get; private set; }

    public Guid JobId { get; private set; }

    public ApplicationStatus Status { get; private set; }

    public decimal? MatchScore { get; private set; }

    public DateTime AppliedAtUtc { get; private set; }

    public DateTime LastActionAtUtc { get; private set; }

    public CandidateProfile CandidateProfile { get; private set; } = null!;

    public Job Job { get; private set; } = null!;

    public ICollection<ApplicationStatusHistory> StatusHistory { get; private set; }
        = new List<ApplicationStatusHistory>();
}