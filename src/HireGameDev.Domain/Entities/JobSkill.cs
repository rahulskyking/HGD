namespace HireGameDev.Domain.Entities;

public class JobSkill : BaseEntity
{
    private JobSkill()
    {
    }

    public JobSkill(Guid jobId, Guid skillId, bool isRequired)
    {
        if (jobId == Guid.Empty)
        {
            throw new ArgumentException("A job is required.", nameof(jobId));
        }

        if (skillId == Guid.Empty)
        {
            throw new ArgumentException("A skill is required.", nameof(skillId));
        }

        JobId = jobId;
        SkillId = skillId;
        IsRequired = isRequired;
    }

    public Guid JobId { get; private set; }

    public Guid SkillId { get; private set; }

    public bool IsRequired { get; private set; } = true;

    public Job Job { get; private set; } = null!;

    public Skill Skill { get; private set; } = null!;
}
