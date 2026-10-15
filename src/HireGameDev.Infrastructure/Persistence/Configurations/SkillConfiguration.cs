using HireGameDev.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireGameDev.Infrastructure.Persistence.Configurations;

public class SkillConfiguration : IEntityTypeConfiguration<Skill>
{
    public void Configure(EntityTypeBuilder<Skill> builder)
    {
        builder.ToTable("Skills");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(Skill.NameMaxLength)
            .IsRequired();

        builder.Property(x => x.Category)
            .HasMaxLength(Skill.CategoryMaxLength)
            .IsRequired();

        builder.HasIndex(x => x.Name)
            .IsUnique();
    }
}