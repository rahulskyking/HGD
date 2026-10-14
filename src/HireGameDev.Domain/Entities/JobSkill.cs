namespace HireGameDev.Domain.Entities;

public class JobSkill : BaseEntity
{
    private JobSkill()
    {
    }

    public JobSkill(Guid jobId, Guid skillId)
    {
        JobId = jobId;
        SkillId = skillId;
    }

    public Guid JobId { get; private set; }

    public Guid SkillId { get; private set; }

    public Job Job { get; private set; } = null!;

    public Skill Skill { get; private set; } = null!;
}