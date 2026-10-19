using HireGameDev.Application.Features.Skills.GetSkills;
using HireGameDev.Application.Tests.Persistence;
using HireGameDev.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HireGameDev.Application.Tests.Features.Skills.GetSkills;

public class GetSkillsHandlerTests : IDisposable
{
    private readonly SqliteDatabase _database = new();

    public void Dispose() => _database.Dispose();

    [Fact]
    public async Task Returns_the_master_list_ordered_by_category_then_name()
    {
        await using (var setup = _database.CreateContext())
        {
            setup.Skills.AddRange(
                new Skill("Unity", SkillCategory.Engines),
                new Skill("Godot", SkillCategory.Engines),
                new Skill("C#", SkillCategory.Languages));
            await setup.SaveChangesAsync();
        }

        await using var context = _database.CreateContext();
        var result = await new GetSkillsHandler(context).HandleAsync(new GetSkillsQuery());

        Assert.Equal(new[] { "Engines:Godot", "Engines:Unity", "Languages:C#" },
            result.Select(x => $"{x.Category}:{x.Name}"));
    }

    [Fact]
    public async Task Returns_an_empty_list_when_the_catalogue_is_empty()
    {
        await using var context = _database.CreateContext();
        var result = await new GetSkillsHandler(context).HandleAsync(new GetSkillsQuery());

        Assert.Empty(result);
    }
}
