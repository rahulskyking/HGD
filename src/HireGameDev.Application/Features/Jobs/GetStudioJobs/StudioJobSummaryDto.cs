using HireGameDev.Domain.Enums;

namespace HireGameDev.Application.Features.Jobs.GetStudioJobs;

public sealed record StudioJobSummaryDto(
    Guid Id,
    string Title,
    JobCategory Category,
    JobType Type,
    SeniorityLevel SeniorityLevel,
    decimal MinimumAnnualSalary,
    decimal MaximumAnnualSalary,
    string City,
    LocationMode LocationMode,
    JobStatus Status,
    DateTime? ExpiresAtUtc,
    DateTime CreatedAtUtc,
    int RequiredSkillCount,
    int NiceToHaveSkillCount);
