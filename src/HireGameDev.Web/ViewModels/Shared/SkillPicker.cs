using HireGameDev.Application.Features.Skills.GetSkills;

namespace HireGameDev.Web.ViewModels.Shared;

/// <summary>Model for the shared skill picker partial.</summary>
public sealed record SkillPicker(
    string FieldName,
    IReadOnlyList<SkillDto> Skills,
    IReadOnlyCollection<Guid> SelectedIds,
    int Minimum,
    int Maximum)
{
    public IEnumerable<IGrouping<string, SkillDto>> Groups => Skills.GroupBy(x => x.Category);
}
