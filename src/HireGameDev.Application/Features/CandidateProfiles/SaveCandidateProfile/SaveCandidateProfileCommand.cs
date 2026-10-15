using HireGameDev.Domain.Entities;
using HireGameDev.Domain.Enums;

namespace HireGameDev.Application.Features.CandidateProfiles.SaveCandidateProfile;

/// <summary>
/// Everything on the edit form, in one go. Creates the profile the first time and updates it
/// afterwards; lists replace what was stored. Salaries are rupees per year.
/// </summary>
public sealed record SaveCandidateProfileCommand(
    Guid UserId,
    string FullName,
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
    IReadOnlyCollection<Guid> SkillIds,
    IReadOnlyCollection<ShippedTitleDetails> ShippedTitles,
    IReadOnlyCollection<PortfolioLinkDetails> PortfolioLinks);
