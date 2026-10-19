using HireGameDev.Application.Features.Skills.GetSkills;

namespace HireGameDev.Web.Pages.Shared;

/// <summary>Model for <c>_SkillPickerPartial</c>.</summary>
/// <param name="FieldName">Form field the chosen skill ids post as, e.g. <c>Input.SkillIds</c>.</param>
/// <param name="Skills">The whole catalogue, already ordered by category then name.</param>
/// <param name="SelectedIds">Ids to render checked.</param>
/// <param name="Minimum">Fewest skills that count as valid; shown in the live counter.</param>
public sealed record SkillPicker(
    string FieldName,
    IReadOnlyList<SkillDto> Skills,
    IReadOnlyCollection<Guid> SelectedIds,
    int Minimum,
    int Maximum)
{
    public IEnumerable<IGrouping<string, SkillDto>> Groups => Skills.GroupBy(x => x.Category);
}
