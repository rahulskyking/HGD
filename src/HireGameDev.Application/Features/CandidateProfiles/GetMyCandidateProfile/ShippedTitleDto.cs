namespace HireGameDev.Application.Features.CandidateProfiles.GetMyCandidateProfile;

public sealed record ShippedTitleDto(
    string GameName,
    string StudioName,
    string Role,
    int Year,
    bool IsVerified);
