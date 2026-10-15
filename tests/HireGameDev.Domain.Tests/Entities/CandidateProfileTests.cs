using HireGameDev.Domain.Entities;
using HireGameDev.Domain.Enums;

namespace HireGameDev.Domain.Tests.Entities;

public class CandidateProfileTests
{
    private static readonly Guid UserId = Guid.NewGuid();

    private static CandidateProfile SavedProfile()
    {
        var profile = new CandidateProfile(UserId);
        profile.UpdateBasics(
            fullName: "  Ananya Krishnan ",
            phone: null,
            city: "Bengaluru",
            jobTitle: "Gameplay Programmer",
            seniorityLevel: SeniorityLevel.Mid,
            yearsOfExperience: 4.5m,
            currentAnnualSalary: 900_000m,
            expectedAnnualSalary: 1_400_000m,
            noticePeriod: NoticePeriod.ThirtyDays,
            openToRemote: true,
            openToWorkStatus: OpenToWorkStatus.OpenToOffers,
            about: null);
        profile.ReplaceSkills([Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()]);
        return profile;
    }

    [Fact]
    public void Requires_an_account()
    {
        Assert.Throws<ArgumentException>(() => new CandidateProfile(Guid.Empty));
    }

    [Fact]
    public void Basics_are_trimmed_and_stored()
    {
        var profile = SavedProfile();

        Assert.Equal("Ananya Krishnan", profile.FullName);
        Assert.Null(profile.Phone);
        Assert.Equal(4.5m, profile.YearsOfExperience);
        Assert.Equal(1_400_000m, profile.ExpectedAnnualSalary);
        Assert.True(profile.OpenToRemote);
        Assert.Null(profile.About);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Full_name_is_required(string fullName)
    {
        var profile = new CandidateProfile(UserId);

        Assert.Throws<ArgumentException>(() => Update(profile, fullName: fullName));
    }

    [Fact]
    public void Full_name_cannot_exceed_its_limit()
    {
        var profile = new CandidateProfile(UserId);

        Assert.Throws<ArgumentException>(() =>
            Update(profile, fullName: new string('a', CandidateProfile.FullNameMaxLength + 1)));
    }

    [Fact]
    public void About_cannot_exceed_its_limit()
    {
        var profile = new CandidateProfile(UserId);

        Assert.Throws<ArgumentException>(() =>
            Update(profile, about: new string('a', CandidateProfile.AboutMaxLength + 1)));
    }

    [Theory]
    [InlineData(-0.5)]
    [InlineData(51)]
    public void Years_of_experience_must_be_in_range(double years)
    {
        var profile = new CandidateProfile(UserId);

        Assert.Throws<ArgumentOutOfRangeException>(() => Update(profile, years: (decimal)years));
    }

    [Fact]
    public void Expected_salary_must_be_positive()
    {
        var profile = new CandidateProfile(UserId);

        Assert.Throws<ArgumentOutOfRangeException>(() => Update(profile, expectedSalary: 0m));
    }

    [Fact]
    public void Undefined_enum_values_are_rejected()
    {
        var profile = new CandidateProfile(UserId);

        Assert.Throws<ArgumentOutOfRangeException>(() => Update(profile, seniority: (SeniorityLevel)99));
    }

    [Fact]
    public void Skills_need_the_minimum_count_after_removing_duplicates()
    {
        var profile = new CandidateProfile(UserId);
        var skill = Guid.NewGuid();

        Assert.Throws<ArgumentException>(() => profile.ReplaceSkills([skill, skill, skill]));
    }

    [Fact]
    public void Skills_cannot_exceed_the_maximum()
    {
        var profile = new CandidateProfile(UserId);
        var tooMany = Enumerable.Range(0, CandidateProfile.MaximumSkillCount + 1).Select(_ => Guid.NewGuid()).ToList();

        Assert.Throws<ArgumentException>(() => profile.ReplaceSkills(tooMany));
    }

    [Fact]
    public void Replacing_skills_keeps_rows_for_skills_that_stay()
    {
        var profile = SavedProfile();
        var kept = profile.CandidateSkills.Take(2).ToList();
        var newSkill = Guid.NewGuid();

        profile.ReplaceSkills([kept[0].SkillId, kept[1].SkillId, newSkill]);

        Assert.Equal(3, profile.CandidateSkills.Count);
        Assert.Contains(kept[0], profile.CandidateSkills);
        Assert.Contains(kept[1], profile.CandidateSkills);
        Assert.Contains(profile.CandidateSkills, x => x.SkillId == newSkill);
    }

    [Fact]
    public void Shipped_titles_are_replaced_as_a_whole()
    {
        var profile = SavedProfile();
        profile.ReplaceShippedTitles([new ShippedTitleDetails("Raji", "Nodding Heads", "Programmer", 2020)]);

        profile.ReplaceShippedTitles([new ShippedTitleDetails("Ludo King", "Gametion", "Programmer", 2023)]);

        var title = Assert.Single(profile.ShippedTitles);
        Assert.Equal("Ludo King", title.GameName);
        Assert.Equal(profile.Id, title.CandidateProfileId);
    }

    [Fact]
    public void Shipped_titles_cannot_exceed_the_maximum()
    {
        var profile = SavedProfile();
        var tooMany = Enumerable.Range(0, CandidateProfile.MaximumShippedTitles + 1)
            .Select(i => new ShippedTitleDetails($"Game {i}", "Studio", "Role", 2020))
            .ToList();

        Assert.Throws<ArgumentException>(() => profile.ReplaceShippedTitles(tooMany));
    }

    [Fact]
    public void Portfolio_links_cannot_exceed_the_maximum()
    {
        var profile = SavedProfile();
        var tooMany = Enumerable.Range(0, CandidateProfile.MaximumPortfolioLinks + 1)
            .Select(i => new PortfolioLinkDetails($"https://example.com/{i}", null))
            .ToList();

        Assert.Throws<ArgumentException>(() => profile.ReplacePortfolioLinks(tooMany));
    }

    [Fact]
    public void Completeness_grows_with_each_optional_section()
    {
        var profile = SavedProfile();
        Assert.Equal(50, profile.CompletenessPercent());

        profile.SetPhoto("/media/photos/a.jpg");
        Assert.Equal(65, profile.CompletenessPercent());

        Update(profile, about: "I ship games.");
        Assert.Equal(75, profile.CompletenessPercent());

        profile.ReplaceShippedTitles([new ShippedTitleDetails("Raji", "Nodding Heads", "Programmer", 2020)]);
        Assert.Equal(90, profile.CompletenessPercent());

        profile.ReplacePortfolioLinks([new PortfolioLinkDetails("https://github.com/ananya", "GitHub")]);
        Assert.Equal(100, profile.CompletenessPercent());

        profile.RemovePhoto();
        Assert.Equal(85, profile.CompletenessPercent());
    }

    [Fact]
    public void Changes_set_the_updated_timestamp()
    {
        var profile = new CandidateProfile(UserId);
        Assert.Null(profile.UpdatedAtUtc);

        Update(profile);

        Assert.NotNull(profile.UpdatedAtUtc);
    }

    private static void Update(
        CandidateProfile profile,
        string fullName = "Ananya Krishnan",
        SeniorityLevel seniority = SeniorityLevel.Mid,
        decimal years = 4m,
        decimal expectedSalary = 1_200_000m,
        string? about = null)
    {
        profile.UpdateBasics(
            fullName,
            phone: "+91 98765 43210",
            city: "Pune",
            jobTitle: "Gameplay Programmer",
            seniorityLevel: seniority,
            yearsOfExperience: years,
            currentAnnualSalary: null,
            expectedAnnualSalary: expectedSalary,
            noticePeriod: NoticePeriod.Immediate,
            openToRemote: false,
            openToWorkStatus: OpenToWorkStatus.ActivelyLooking,
            about: about);
    }
}
