using HireGameDev.Domain.Enums;

namespace HireGameDev.Application.Features.Jobs.SaveStudioJob;

public sealed record SaveStudioJobCommand(
    Guid UserId,
    Guid? JobId,
    string Title,
    JobCategory Category,
    JobType Type,
    SeniorityLevel SeniorityLevel,
    decimal MinimumAnnualSalary,
    decimal MaximumAnnualSalary,
    LocationMode LocationMode,
    string City,
    string Description,
    IReadOnlyCollection<Guid> RequiredSkillIds,
    IReadOnlyCollection<Guid> NiceToHaveSkillIds);
