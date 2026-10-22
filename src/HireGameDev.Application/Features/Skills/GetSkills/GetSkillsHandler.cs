using HireGameDev.Application.Persistence;

namespace HireGameDev.Application.Features.Skills.GetSkills;

public sealed class GetSkillsHandler(ISkillRepository skills)
{
    public async Task<IReadOnlyList<SkillDto>> HandleAsync(
        GetSkillsQuery query,
        CancellationToken cancellationToken = default)
    {
        var catalog = await skills.GetAllAsync(cancellationToken);

        return catalog
            .OrderBy(skill => skill.Category)
            .ThenBy(skill => skill.Name)
            .Select(skill => new SkillDto(skill.Id, skill.Name, skill.Category))
            .ToList();
    }
}
