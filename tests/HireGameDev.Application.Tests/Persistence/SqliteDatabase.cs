using HireGameDev.Domain.Entities;
using HireGameDev.Infrastructure.Identity;
using HireGameDev.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace HireGameDev.Application.Tests.Persistence;

/// <summary>
/// A fresh in-memory SQLite database with the real EF model, alive for as long as the
/// connection is open. Each test creates its own, so tests never share state.
/// Every context from <see cref="CreateContext"/> sees the same database, which lets a
/// test write with one context and read back with another, like separate requests.
/// </summary>
internal sealed class SqliteDatabase : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<ApplicationDbContext> _options;

    public SqliteDatabase()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .Options;

        using var context = CreateContext();
        context.Database.EnsureCreated();
    }

    public ApplicationDbContext CreateContext() => new(_options);

    /// <summary>Profiles reference an Identity user row, so tests need one to exist.</summary>
    public async Task<Guid> AddUserAsync(string email = "dev@example.com")
    {
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = email,
            NormalizedUserName = email.ToUpperInvariant(),
            Email = email,
            NormalizedEmail = email.ToUpperInvariant(),
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString("N"),
        };

        await using var context = CreateContext();
        context.Users.Add(user);
        await context.SaveChangesAsync();

        return user.Id;
    }

    public async Task<IReadOnlyList<Skill>> AddSkillsAsync(params string[] names)
    {
        var skills = names.Select(name => new Skill(name, "Engines")).ToList();

        await using var context = CreateContext();
        context.Skills.AddRange(skills);
        await context.SaveChangesAsync();

        return skills;
    }

    public void Dispose() => _connection.Dispose();
}
