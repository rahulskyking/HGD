namespace HireGameDev.Domain.Entities;

public class ShippedTitle : BaseEntity
{
    private ShippedTitle()
    {
    }

    public ShippedTitle(
        Guid candidateProfileId,
        string gameName,
        string studioName,
        string role,
        int year)
    {
        CandidateProfileId = candidateProfileId;
        GameName = gameName;
        StudioName = studioName;
        Role = role;
        Year = year;
    }

    public Guid CandidateProfileId { get; private set; }

    public string GameName { get; private set; } = string.Empty;

    public string StudioName { get; private set; } = string.Empty;

    public string Role { get; private set; } = string.Empty;

    public int Year { get; private set; }

    public bool IsVerified { get; private set; }

    public CandidateProfile CandidateProfile { get; private set; } = null!;
}