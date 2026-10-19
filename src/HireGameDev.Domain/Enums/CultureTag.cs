namespace HireGameDev.Domain.Enums;

/// <summary>
/// Claims a studio makes about how it works (spec §7.3 "No Crunch, Remote-friendly, …").
/// A closed list so candidates can filter on them later; stored as numbers, one row per tag.
/// </summary>
public enum CultureTag
{
    NoCrunch = 1,
    RemoteFriendly = 2,
    HybridWork = 3,
    FlexibleHours = 4,
    FiveDayWeek = 5,
    HealthInsurance = 6,
    Esops = 7,
    LearningBudget = 8,
    HiresFreshers = 9,
    RelocationSupport = 10,
    WomenLed = 11,
    Mentorship = 12,
}
