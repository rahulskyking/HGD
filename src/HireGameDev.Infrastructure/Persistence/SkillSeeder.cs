using HireGameDev.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HireGameDev.Infrastructure.Persistence;

public static class SkillSeeder
{
    /// <summary>
    /// Brings the Skills table in line with <see cref="SkillCatalog"/>: adds missing skills and
    /// moves re-categorised ones. Never deletes — a skill may already be on profiles or jobs.
    /// Safe to run on every start.
    /// </summary>
    public static async Task SeedAsync(IServiceProvider services)
    {
        var dbContext = services.GetRequiredService<ApplicationDbContext>();

        var existing = await dbContext.Skills.ToDictionaryAsync(x => x.Name);

        foreach (var (name, category) in SkillCatalog.Entries)
        {
            if (!existing.TryGetValue(name, out var skill))
            {
                dbContext.Skills.Add(new Skill(name, category));
            }
            else if (skill.Category != category)
            {
                skill.MoveToCategory(category);
            }
        }

        await dbContext.SaveChangesAsync();
    }
}
