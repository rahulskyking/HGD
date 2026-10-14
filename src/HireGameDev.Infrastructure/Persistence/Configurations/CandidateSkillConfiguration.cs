using HireGameDev.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireGameDev.Infrastructure.Persistence.Configurations;

public class CandidateSkillConfiguration : IEntityTypeConfiguration<CandidateSkill>
{
    public void Configure(EntityTypeBuilder<CandidateSkill> builder)
    {
        builder.ToTable("CandidateSkills");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Proficiency)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.CandidateProfileId,
            x.SkillId
        })
        .IsUnique();

        builder.HasOne(x => x.CandidateProfile)
            .WithMany(x => x.CandidateSkills)
            .HasForeignKey(x => x.CandidateProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Skill)
            .WithMany(x => x.CandidateSkills)
            .HasForeignKey(x => x.SkillId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}