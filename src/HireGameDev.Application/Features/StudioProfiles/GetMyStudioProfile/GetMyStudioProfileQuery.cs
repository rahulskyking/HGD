namespace HireGameDev.Application.Features.StudioProfiles.GetMyStudioProfile;

/// <summary>The signed-in studio's own profile; <c>null</c> result means it has not been created yet.</summary>
public sealed record GetMyStudioProfileQuery(Guid UserId);
