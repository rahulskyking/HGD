using HireGameDev.Application.Features.CandidateProfiles.GetMyCandidateProfile;
using HireGameDev.Application.Tests.Persistence;
using HireGameDev.Domain.Entities;
using HireGameDev.Domain.Enums;

namespace HireGameDev.Application.Tests.Features.CandidateProfiles.GetMyCandidateProfile;

public class GetMyCandidateProfileHandlerTests : IDisposable
{
    private readonly SqliteDatabase _database = new();

    public void Dispose() => _database.Dispose();

    [Fact]
    public async Task Returns_null_when_the_candidate_has_no_profile_yet()
    {
        var userId = await _database.AddUserAsync();

        await using var context = _database.CreateContext();
        var result = await new GetMyCandidateProfileHandler(context).HandleAsync(new GetMyCandidateProfileQuery(userId));

        Assert.Null(result);
    }

    [Fact]
    public async Task Maps_the_profile_with_its_skills_titles_and_links()
    {
        var userId = await _database.AddUserAsync();
        var otherUserId = await _database.AddUserAsync("other@example.com");
        var skills = await _database.AddSkillsAsync("Unity", "C#", "Godot");

        await using (var setup = _database.CreateContext())
        {
            setup.CandidateProfiles.Add(Profile(userId, skills, "Ananya Krishnan"));
            setup.CandidateProfiles.Add(Profile(otherUserId, skills, "Someone Else"));
            await setup.SaveChangesAsync();
        }

        await using var context = _database.CreateContext();
        var result = await new GetMyCandidateProfileHandler(context).HandleAsync(new GetMyCandidateProfileQuery(userId));

        Assert.NotNull(result);
        Assert.Equal("Ananya Krishnan", result.FullName);
        Assert.Equal(new[] { "C#", "Godot", "Unity" }, result.Skills.Select(x => x.Name));
        Assert.Equal("Engines", result.Skills[0].Category);
        Assert.Equal(2023, result.ShippedTitles[0].Year);
        Assert.Equal(2020, result.ShippedTitles[1].Year);
        Assert.Equal("GitHub", result.PortfolioLinks.Single().Label);
        Assert.Equal(85, result.CompletenessPercent);
    }

    private static CandidateProfile Profile(Guid userId, IReadOnlyList<Skill> skills, string fullName)
    {
        var profile = new CandidateProfile(userId);
        profile.UpdateBasics(
            fullName, null, "Pune", "Gameplay Programmer", SeniorityLevel.Mid, 4m, null, 1_200_000m,
            NoticePeriod.ThirtyDays, true, OpenToWorkStatus.OpenToOffers, "About me.");
        profile.ReplaceSkills(skills.Select(x => x.Id).ToList());
        profile.ReplaceShippedTitles(
        [
            new ShippedTitleDetails("Raji: An Ancient Epic", "Nodding Heads Games", "Programmer", 2020),
            new ShippedTitleDetails("Ludo King", "Gametion", "Programmer", 2023),
        ]);
        profile.ReplacePortfolioLinks([new PortfolioLinkDetails("https://github.com/ananya", "GitHub")]);

        return profile;
    }
}
