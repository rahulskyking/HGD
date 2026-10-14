namespace HireGameDev.Domain.Entities;

public class CandidateSkill : BaseEntity
{
    private CandidateSkill()
    {
    }

    public CandidateSkill(
        Guid candidateProfileId,
        Guid skillId)
    {
        CandidateProfileId = candidateProfileId;
        SkillId = skillId;
    }

    public Guid CandidateProfileId { get; private set; }

    public Guid SkillId { get; private set; }

    public int Proficiency { get; private set; }

    public CandidateProfile CandidateProfile { get; private set; } = null!;

    public Skill Skill { get; private set; } = null!;
}