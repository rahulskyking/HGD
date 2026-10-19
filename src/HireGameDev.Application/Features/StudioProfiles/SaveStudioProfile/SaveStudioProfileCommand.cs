using HireGameDev.Domain.Entities;
using HireGameDev.Domain.Enums;

namespace HireGameDev.Application.Features.StudioProfiles.SaveStudioProfile;

/// <summary>
/// Everything on the studio edit form in one go; creates the profile on first save. Engines
/// are skill ids from the Engines category; genres and culture tags are closed enum values.
/// Lists replace what was saved before.
/// </summary>
public sealed record SaveStudioProfileCommand(
    Guid UserId,
    string Name,
    string? WebsiteUrl,
    string City,
    StudioSize Size,
    StudioType Type,
    string? About,
    string? GstNumber,
    IReadOnlyCollection<Guid> EngineIds,
    IReadOnlyCollection<GameGenre> Genres,
    IReadOnlyCollection<CultureTag> CultureTags,
    IReadOnlyCollection<StudioGameDetails> Games);
