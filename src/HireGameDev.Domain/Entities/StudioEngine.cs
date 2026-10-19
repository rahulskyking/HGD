namespace HireGameDev.Domain.Entities;

/// <summary>One studio engine, linked to the shared Skills master list for future job filters.</summary>
public class StudioEngine : BaseEntity
{
    private StudioEngine()
    {
    }

    public StudioEngine(Guid studioProfileId, Guid skillId)
    {
        if (studioProfileId == Guid.Empty)
        {
            throw new ArgumentException("An engine must belong to a studio profile.", nameof(studioProfileId));
        }

        if (skillId == Guid.Empty)
        {
            throw new ArgumentException("A skill is required.", nameof(skillId));
        }

        StudioProfileId = studioProfileId;
        SkillId = skillId;
    }

    public Guid StudioProfileId { get; private set; }

    public Guid SkillId { get; private set; }

    public StudioProfile StudioProfile { get; private set; } = null!;

    public Skill Skill { get; private set; } = null!;
}
