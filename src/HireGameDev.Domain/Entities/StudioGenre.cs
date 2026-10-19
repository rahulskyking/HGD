using HireGameDev.Domain.Enums;

namespace HireGameDev.Domain.Entities;

/// <summary>One studio genre, stored as a closed enum value on its own join table.</summary>
public class StudioGenre : BaseEntity
{
    private StudioGenre()
    {
    }

    public StudioGenre(Guid studioProfileId, GameGenre genre)
    {
        if (studioProfileId == Guid.Empty)
        {
            throw new ArgumentException("A genre must belong to a studio profile.", nameof(studioProfileId));
        }

        if (!Enum.IsDefined(genre))
        {
            throw new ArgumentOutOfRangeException(nameof(genre), $"{genre} is not a valid {nameof(GameGenre)}.");
        }

        StudioProfileId = studioProfileId;
        Genre = genre;
    }

    public Guid StudioProfileId { get; private set; }

    public GameGenre Genre { get; private set; }

    public StudioProfile StudioProfile { get; private set; } = null!;
}
