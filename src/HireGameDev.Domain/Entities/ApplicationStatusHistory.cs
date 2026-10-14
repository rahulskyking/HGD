using HireGameDev.Domain.Enums;

namespace HireGameDev.Domain.Entities;

public class ApplicationStatusHistory : BaseEntity
{
    private ApplicationStatusHistory()
    {
    }

    public ApplicationStatusHistory(
        Guid applicationId,
        ApplicationStatus? fromStatus,
        ApplicationStatus toStatus)
    {
        ApplicationId = applicationId;
        FromStatus = fromStatus;
        ToStatus = toStatus;
        ChangedAtUtc = DateTime.UtcNow;
    }

    public Guid ApplicationId { get; private set; }

    public ApplicationStatus? FromStatus { get; private set; }

    public ApplicationStatus ToStatus { get; private set; }

    public DateTime ChangedAtUtc { get; private set; }

    public Application Application { get; private set; } = null!;
}