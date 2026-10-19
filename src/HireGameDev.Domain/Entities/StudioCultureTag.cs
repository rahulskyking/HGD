using HireGameDev.Domain.Enums;

namespace HireGameDev.Domain.Entities;

/// <summary>One culture tag a studio claims; one row per tag so candidates can filter on them later.</summary>
public class StudioCultureTag : BaseEntity
{
    private StudioCultureTag()
    {
    }

    public StudioCultureTag(Guid studioProfileId, CultureTag tag)
    {
        if (studioProfileId == Guid.Empty)
        {
            throw new ArgumentException("A culture tag must belong to a profile.", nameof(studioProfileId));
        }

        if (!Enum.IsDefined(tag))
        {
            throw new ArgumentOutOfRangeException(nameof(tag), $"{tag} is not a valid {nameof(CultureTag)}.");
        }

        StudioProfileId = studioProfileId;
        Tag = tag;
    }

    public Guid StudioProfileId { get; private set; }

    public CultureTag Tag { get; private set; }

    public StudioProfile StudioProfile { get; private set; } = null!;
}
