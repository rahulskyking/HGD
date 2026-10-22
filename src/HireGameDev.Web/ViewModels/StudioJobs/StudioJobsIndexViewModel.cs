using HireGameDev.Application.Features.Jobs.GetStudioJobs;

namespace HireGameDev.Web.ViewModels.StudioJobs;

public sealed class StudioJobsIndexViewModel
{
    public bool HasStudioProfile { get; init; }

    public IReadOnlyList<StudioJobSummaryDto> Jobs { get; init; } = [];

    public string? StatusMessage { get; init; }

    public string? ErrorMessage { get; init; }
}
