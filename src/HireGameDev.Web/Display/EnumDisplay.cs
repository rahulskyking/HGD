using HireGameDev.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HireGameDev.Web.Display;

/// <summary>
/// Human wording for Domain enums. The Domain keeps code names; what the user reads is a
/// Web concern, so every page gets the same label from here.
/// </summary>
public static class EnumDisplay
{
    public static string Name(SeniorityLevel value) => value switch
    {
        SeniorityLevel.Fresher => "Fresher (0 years)",
        SeniorityLevel.Junior => "Junior (1–3 years)",
        SeniorityLevel.Mid => "Mid-level (3–7 years)",
        SeniorityLevel.Senior => "Senior (7–12 years)",
        SeniorityLevel.Lead => "Lead / Director (12+ years)",
        _ => value.ToString(),
    };

    public static string Name(NoticePeriod value) => value switch
    {
        NoticePeriod.Immediate => "Immediate",
        NoticePeriod.FifteenDays => "15 days",
        NoticePeriod.ThirtyDays => "30 days",
        NoticePeriod.SixtyDays => "60 days",
        NoticePeriod.NinetyDays => "90 days",
        _ => value.ToString(),
    };

    public static string Name(OpenToWorkStatus value) => value switch
    {
        OpenToWorkStatus.ActivelyLooking => "Actively looking",
        OpenToWorkStatus.OpenToOffers => "Open to offers",
        OpenToWorkStatus.NotLooking => "Not looking right now",
        _ => value.ToString(),
    };

    public static string Name(StudioSize value) => value switch
    {
        StudioSize.UpTo10 => "1–10 people",
        StudioSize.From11To50 => "11–50 people",
        StudioSize.From51To200 => "51–200 people",
        StudioSize.Over200 => "200+ people",
        _ => value.ToString(),
    };

    public static string Name(StudioType value) => value switch
    {
        StudioType.GameStudio => "Game studio",
        StudioType.Outsourcing => "Outsourcing / co-development",
        StudioType.Publisher => "Publisher",
        StudioType.Indie => "Indie studio",
        _ => value.ToString(),
    };

    public static string Name(GameGenre value) => value switch
    {
        GameGenre.MobileCasual => "Mobile casual",
        GameGenre.HyperCasual => "Hyper-casual",
        GameGenre.Puzzle => "Puzzle",
        GameGenre.Match3 => "Match-3",
        GameGenre.IdleAndClicker => "Idle & clicker",
        GameGenre.Rpg => "RPG",
        GameGenre.ActionRpg => "Action RPG",
        GameGenre.Mmorpg => "MMORPG",
        GameGenre.Fps => "FPS",
        GameGenre.ThirdPersonShooter => "Third-person shooter",
        GameGenre.BattleRoyale => "Battle royale",
        GameGenre.Moba => "MOBA",
        GameGenre.Roguelike => "Roguelike",
        GameGenre.Platformer => "Platformer",
        GameGenre.RealTimeStrategy => "Real-time strategy",
        GameGenre.TurnBasedStrategy => "Turn-based strategy",
        GameGenre.Simulation => "Simulation",
        GameGenre.Racing => "Racing",
        GameGenre.Sports => "Sports",
        GameGenre.Fighting => "Fighting",
        GameGenre.Horror => "Horror",
        GameGenre.Survival => "Survival",
        GameGenre.OpenWorld => "Open world",
        GameGenre.NarrativeAdventure => "Narrative adventure",
        GameGenre.VisualNovel => "Visual novel",
        GameGenre.Casino => "Casino",
        GameGenre.CardAndBoardGames => "Card & board games",
        GameGenre.EdTechAndSeriousGames => "EdTech & serious games",
        _ => value.ToString(),
    };

    public static string Name(CultureTag value) => value switch
    {
        CultureTag.NoCrunch => "No crunch",
        CultureTag.RemoteFriendly => "Remote-friendly",
        CultureTag.HybridWork => "Hybrid work",
        CultureTag.FlexibleHours => "Flexible hours",
        CultureTag.FiveDayWeek => "5-day week",
        CultureTag.HealthInsurance => "Health insurance",
        CultureTag.Esops => "ESOPs",
        CultureTag.LearningBudget => "Learning budget",
        CultureTag.HiresFreshers => "Hires freshers",
        CultureTag.RelocationSupport => "Relocation support",
        CultureTag.WomenLed => "Women-led",
        CultureTag.Mentorship => "Mentorship",
        _ => value.ToString(),
    };

    public static IReadOnlyList<SelectListItem> StudioSizes() =>
        Enum.GetValues<StudioSize>().Select(x => new SelectListItem(Name(x), x.ToString())).ToList();

    public static IReadOnlyList<SelectListItem> StudioTypes() =>
        Enum.GetValues<StudioType>().Select(x => new SelectListItem(Name(x), x.ToString())).ToList();

    public static IReadOnlyList<SelectListItem> SeniorityLevels() =>
        Enum.GetValues<SeniorityLevel>().Select(x => new SelectListItem(Name(x), x.ToString())).ToList();

    public static IReadOnlyList<SelectListItem> NoticePeriods() =>
        Enum.GetValues<NoticePeriod>().Select(x => new SelectListItem(Name(x), x.ToString())).ToList();

    public static IReadOnlyList<SelectListItem> OpenToWorkStatuses() =>
        Enum.GetValues<OpenToWorkStatus>().Select(x => new SelectListItem(Name(x), x.ToString())).ToList();
}
