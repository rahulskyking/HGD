using HireGameDev.Domain.Entities;
using HireGameDev.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HireGameDev.Application.Tests.Persistence;

/// <summary>
/// Lives here rather than in an Infrastructure test project because the seeder is the only
/// Infrastructure code with logic worth a test today; split it out when that changes.
/// </summary>
public class SkillSeederTests : IDisposable
{
    private readonly SqliteDatabase _database = new();

    public void Dispose() => _database.Dispose();

    [Fact]
    public void Catalogue_names_are_unique_and_within_limits()
    {
        var names = SkillCatalog.Entries.Select(x => x.Name).ToList();

        Assert.Equal(names.Count, names.Distinct(StringComparer.OrdinalIgnoreCase).Count());
        Assert.All(SkillCatalog.Entries, entry =>
        {
            Assert.InRange(entry.Name.Length, 1, Skill.NameMaxLength);
            Assert.InRange(entry.Category.Length, 1, Skill.CategoryMaxLength);
        });
        Assert.Equal(8, SkillCatalog.Entries.Select(x => x.Category).Distinct().Count());
    }

    [Fact]
    public async Task Seeds_every_catalogue_entry_once_and_is_safe_to_rerun()
    {
        await SeedAsync();
        await SeedAsync();

        await using var context = _database.CreateContext();
        Assert.Equal(SkillCatalog.Entries.Count, await context.Skills.CountAsync());
    }

    [Fact]
    public async Task Moves_a_skill_whose_category_changed_without_recreating_it()
    {
        var existing = (await _database.AddSkillsAsync("Unity")).Single(); // seeded under "Engines" by the helper

        await using (var setup = _database.CreateContext())
        {
            var unity = await setup.Skills.SingleAsync(x => x.Name == "Unity");
            unity.MoveToCategory("Old Category");
            await setup.SaveChangesAsync();
        }

        await SeedAsync();

        await using var context = _database.CreateContext();
        var reseeded = await context.Skills.SingleAsync(x => x.Name == "Unity");
        Assert.Equal(existing.Id, reseeded.Id);
        Assert.Equal(SkillCategory.Engines, reseeded.Category);
    }

    private async Task SeedAsync()
    {
        await using var context = _database.CreateContext();
        var services = new ServiceCollection().AddSingleton(context).BuildServiceProvider();

        await SkillSeeder.SeedAsync(services);
    }
}
