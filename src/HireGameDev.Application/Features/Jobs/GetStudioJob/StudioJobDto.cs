using HireGameDev.Domain.Enums;

namespace HireGameDev.Application.Features.Jobs.GetStudioJob;

public sealed record StudioJobDto(
    Guid Id,
    string Title,
    JobCategory Category,
    JobType Type,
    SeniorityLevel SeniorityLevel,
    decimal MinimumAnnualSalary,
    decimal MaximumAnnualSalary,
    string City,
    string Description,
    LocationMode LocationMode,
    JobStatus Status,
    DateTime? ExpiresAtUtc,
    IReadOnlyList<StudioJobSkillDto> Skills);
