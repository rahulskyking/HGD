namespace HireGameDev.Application.Features.Skills.GetSkills;

public sealed record SkillDto(
    Guid Id,
    string Name,
    string Category);
