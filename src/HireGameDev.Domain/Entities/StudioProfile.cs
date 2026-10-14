namespace HireGameDev.Domain.Entities;

public class StudioProfile : BaseEntity
{
    private StudioProfile()
    {
    }

    public StudioProfile(Guid userId)
    {
        UserId = userId;
    }

    public Guid UserId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string? LogoUrl { get; private set; }

    public string? WebsiteUrl { get; private set; }

    public string City { get; private set; } = string.Empty;

    public string State { get; private set; } = string.Empty;

    public string Size { get; private set; } = string.Empty;

    public string Type { get; private set; } = string.Empty;

    public string? GstNumber { get; private set; }

    public bool IsVerified { get; private set; }

    public ICollection<Job> Jobs { get; private set; }
        = new List<Job>();
}