namespace HireGameDev.Application.Features.Jobs.GetStudioJob;

public sealed record StudioJobSkillDto(
    Guid SkillId,
    string Name,
    string Category,
    bool IsRequired);
