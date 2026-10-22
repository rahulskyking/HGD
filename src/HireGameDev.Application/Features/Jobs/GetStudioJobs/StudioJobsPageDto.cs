namespace HireGameDev.Application.Features.Jobs.GetStudioJobs;

public sealed record StudioJobsPageDto(
    bool HasStudioProfile,
    IReadOnlyList<StudioJobSummaryDto> Jobs);
