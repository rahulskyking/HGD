namespace HireGameDev.Application.Features.Jobs.ChangeStudioJobStatus;

public sealed record ChangeStudioJobStatusCommand(
    Guid UserId,
    Guid JobId,
    StudioJobLifecycleAction Action,
    DateTime? ExpiresAtUtc = null);
