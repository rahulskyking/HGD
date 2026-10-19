using HireGameDev.Application.Features.Skills.GetSkills;
using HireGameDev.Domain.Enums;

namespace HireGameDev.Application.Features.StudioProfiles.GetMyStudioProfile;

/// <summary>
/// Read model for the signed-in studio's own profile. It includes GSTIN for edit-form
/// prefill; public studio-page queries must use a projection that omits it.
/// </summary>
public sealed record StudioProfileDto(
    Guid Id,
    string Name,
    string? LogoUrl,
    string? WebsiteUrl,
    string City,
    string State,
    StudioSize Size,
    StudioType Type,
    string? About,
    string? GstNumber,
    bool IsVerified,
    IReadOnlyList<SkillDto> Engines,
    IReadOnlyList<GameGenre> Genres,
    IReadOnlyList<CultureTag> CultureTags,
    IReadOnlyList<StudioGameDto> Games,
    int CompletenessPercent,
    DateTime LastChangedAtUtc);
