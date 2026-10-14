using HireGameDev.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireGameDev.Infrastructure.Persistence.Configurations;

public class JobSkillConfiguration : IEntityTypeConfiguration<JobSkill>
{
    public void Configure(EntityTypeBuilder<JobSkill> builder)
    {
        builder.ToTable("JobSkills");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new
        {
            x.JobId,
            x.SkillId
        })
        .IsUnique();

        builder.HasOne(x => x.Job)
            .WithMany(x => x.JobSkills)
            .HasForeignKey(x => x.JobId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Skill)
            .WithMany(x => x.JobSkills)
            .HasForeignKey(x => x.SkillId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}