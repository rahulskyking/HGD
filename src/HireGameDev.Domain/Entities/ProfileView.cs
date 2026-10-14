namespace HireGameDev.Domain.Entities;

public class ProfileView : BaseEntity
{
    private ProfileView()
    {
    }

    public ProfileView(
        Guid viewerStudioId,
        Guid candidateProfileId)
    {
        ViewerStudioId = viewerStudioId;
        CandidateProfileId = candidateProfileId;
        ViewedAtUtc = DateTime.UtcNow;
    }

    public Guid ViewerStudioId { get; private set; }

    public Guid CandidateProfileId { get; private set; }

    public DateTime ViewedAtUtc { get; private set; }

    public StudioProfile ViewerStudio { get; private set; } = null!;

    public CandidateProfile CandidateProfile { get; private set; } = null!;
}