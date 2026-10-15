namespace HireGameDev.Application.Features.CandidateProfiles.GetMyCandidateProfile;

/// <summary>The signed-in candidate's own profile, or <c>null</c> when they have not created one yet.</summary>
public sealed record GetMyCandidateProfileQuery(Guid UserId);
