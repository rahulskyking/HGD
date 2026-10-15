using HireGameDev.Domain.Enums;

namespace HireGameDev.Application.Features.CandidateProfiles.GetMyCandidateProfile;

/// <summary>Read model of a profile. Money is rupees per year, exactly as stored.</summary>
public sealed record CandidateProfileDto(
    Guid Id,
    string FullName,
    string? PhotoUrl,
    string? Phone,
    string City,
    string JobTitle,
    SeniorityLevel SeniorityLevel,
    decimal YearsOfExperience,
    decimal? CurrentAnnualSalary,
    decimal ExpectedAnnualSalary,
    NoticePeriod NoticePeriod,
    bool OpenToRemote,
    OpenToWorkStatus OpenToWorkStatus,
    string? About,
    IReadOnlyList<ProfileSkillDto> Skills,
    IReadOnlyList<ShippedTitleDto> ShippedTitles,
    IReadOnlyList<PortfolioLinkDto> PortfolioLinks,
    int CompletenessPercent,
    DateTime LastChangedAtUtc);
