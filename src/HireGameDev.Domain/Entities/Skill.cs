namespace HireGameDev.Domain.Entities;

public class Skill : BaseEntity
{
    private Skill()
    {
    }

    public Skill(string name, string category)
    {
        Name = name;
        Category = category;
    }

    public string Name { get; private set; } = string.Empty;

    public string Category { get; private set; } = string.Empty;

    public ICollection<CandidateSkill> CandidateSkills { get; private set; }
        = new List<CandidateSkill>();

    public ICollection<JobSkill> JobSkills { get; private set; }
        = new List<JobSkill>();
}