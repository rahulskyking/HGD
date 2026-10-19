using HireGameDev.Application.Features.CandidateProfiles.SaveCandidateProfile;
using HireGameDev.Application.Tests.Persistence;
using HireGameDev.Domain.Entities;
using HireGameDev.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HireGameDev.Application.Tests.Features.CandidateProfiles.SaveCandidateProfile;

public class SaveCandidateProfileHandlerTests : IDisposable
{
    private readonly SqliteDatabase _database = new();

    public void Dispose() => _database.Dispose();

    [Fact]
    public async Task Creates_the_profile_on_first_save()
    {
        var userId = await _database.AddUserAsync();
        var skills = await _database.AddSkillsAsync("Unity", "C#", "Godot");

        var result = await Handle(Command(userId, skills.Select(x => x.Id).ToList()));

        Assert.True(result.Succeeded);

        await using var context = _database.CreateContext();
        var profile = await context.CandidateProfiles
            .Include(x => x.CandidateSkills)
            .Include(x => x.ShippedTitles)
            .Include(x => x.PortfolioLinks)
            .SingleAsync(x => x.UserId == userId);

        Assert.Equal("Ananya Krishnan", profile.FullName);
        Assert.Equal(1_400_000m, profile.ExpectedAnnualSalary);
        Assert.Equal(3, profile.CandidateSkills.Count);
        Assert.Single(profile.ShippedTitles);
        Assert.Single(profile.PortfolioLinks);
    }

    [Fact]
    public async Task Second_save_updates_the_same_profile_and_replaces_its_lists()
    {
        var userId = await _database.AddUserAsync();
        var skills = await _database.AddSkillsAsync("Unity", "C#", "Godot", "Unreal Engine 5");
        var ids = skills.Select(x => x.Id).ToList();
        await Handle(Command(userId, ids.Take(3).ToList()));

        var result = await Handle(Command(userId, ids.Skip(1).ToList()) with
        {
            FullName = "Ananya K.",
            ShippedTitles = [],
            PortfolioLinks =
            [
                new PortfolioLinkDetails("https://ananya.dev", null),
                new PortfolioLinkDetails("https://github.com/ananya", "GitHub"),
            ],
        });

        Assert.True(result.Succeeded);

        await using var context = _database.CreateContext();
        Assert.Equal(1, await context.CandidateProfiles.CountAsync());

        var profile = await context.CandidateProfiles
            .Include(x => x.CandidateSkills)
            .Include(x => x.ShippedTitles)
            .Include(x => x.PortfolioLinks)
            .SingleAsync();

        Assert.Equal("Ananya K.", profile.FullName);
        Assert.Equal(ids.Skip(1).OrderBy(x => x), profile.CandidateSkills.Select(x => x.SkillId).OrderBy(x => x));
        Assert.Empty(profile.ShippedTitles);
        Assert.Equal(2, profile.PortfolioLinks.Count);
        Assert.Equal(0, await context.ShippedTitles.CountAsync());
    }

    [Fact]
    public async Task Fails_when_a_skill_does_not_exist()
    {
        var userId = await _database.AddUserAsync();
        var skills = await _database.AddSkillsAsync("Unity", "C#");

        var result = await Handle(Command(userId, [.. skills.Select(x => x.Id), Guid.NewGuid()]));

        Assert.False(result.Succeeded);
        Assert.Single(result.Errors);

        await using var context = _database.CreateContext();
        Assert.Equal(0, await context.CandidateProfiles.CountAsync());
    }

    [Fact]
    public async Task Fails_with_a_message_when_a_domain_rule_is_broken()
    {
        var userId = await _database.AddUserAsync();
        var skills = await _database.AddSkillsAsync("Unity", "C#");

        var result = await Handle(Command(userId, skills.Select(x => x.Id).ToList()));

        Assert.False(result.Succeeded);
        Assert.NotEmpty(result.Errors.Single());

        await using var context = _database.CreateContext();
        Assert.Equal(0, await context.CandidateProfiles.CountAsync());
    }

    private async Task<OperationResult> Handle(SaveCandidateProfileCommand command)
    {
        await using var context = _database.CreateContext();
        var handler = new SaveCandidateProfileHandler(context);

        return await handler.HandleAsync(command);
    }

    private static SaveCandidateProfileCommand Command(Guid userId, IReadOnlyCollection<Guid> skillIds) => new(
        userId,
        FullName: "Ananya Krishnan",
        Phone: "+91 98765 43210",
        City: "Bengaluru",
        JobTitle: "Gameplay Programmer",
        SeniorityLevel: SeniorityLevel.Mid,
        YearsOfExperience: 4.5m,
        CurrentAnnualSalary: 900_000m,
        ExpectedAnnualSalary: 1_400_000m,
        NoticePeriod: NoticePeriod.ThirtyDays,
        OpenToRemote: true,
        OpenToWorkStatus: OpenToWorkStatus.OpenToOffers,
        About: "I ship games.",
        SkillIds: skillIds,
        ShippedTitles: [new ShippedTitleDetails("Raji: An Ancient Epic", "Nodding Heads Games", "Programmer", 2020)],
        PortfolioLinks: [new PortfolioLinkDetails("https://www.artstation.com/ananya", "ArtStation")]);
}
