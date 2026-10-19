using HireGameDev.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireGameDev.Infrastructure.Persistence.Configurations;

public class StudioEngineConfiguration : IEntityTypeConfiguration<StudioEngine>
{
    public void Configure(EntityTypeBuilder<StudioEngine> builder)
    {
        builder.ToTable("StudioEngines");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new
        {
            x.StudioProfileId,
            x.SkillId
        })
        .IsUnique();

        builder.HasOne(x => x.StudioProfile)
            .WithMany(x => x.Engines)
            .HasForeignKey(x => x.StudioProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        // Skill has no navigation back to studios; nothing needs to walk that way.
        builder.HasOne(x => x.Skill)
            .WithMany()
            .HasForeignKey(x => x.SkillId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
