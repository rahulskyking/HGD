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

    public static IReadOnlyList<SelectListItem> SeniorityLevels() =>
        Enum.GetValues<SeniorityLevel>().Select(x => new SelectListItem(Name(x), x.ToString())).ToList();

    public static IReadOnlyList<SelectListItem> NoticePeriods() =>
        Enum.GetValues<NoticePeriod>().Select(x => new SelectListItem(Name(x), x.ToString())).ToList();

    public static IReadOnlyList<SelectListItem> OpenToWorkStatuses() =>
        Enum.GetValues<OpenToWorkStatus>().Select(x => new SelectListItem(Name(x), x.ToString())).ToList();
}
