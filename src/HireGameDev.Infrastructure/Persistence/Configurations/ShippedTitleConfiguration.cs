using HireGameDev.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireGameDev.Infrastructure.Persistence.Configurations;

public class ShippedTitleConfiguration : IEntityTypeConfiguration<ShippedTitle>
{
    public void Configure(EntityTypeBuilder<ShippedTitle> builder)
    {
        builder.ToTable("ShippedTitles");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.GameName)
            .HasMaxLength(ShippedTitle.GameNameMaxLength)
            .IsRequired();

        builder.Property(x => x.StudioName)
            .HasMaxLength(ShippedTitle.StudioNameMaxLength)
            .IsRequired();

        builder.Property(x => x.Role)
            .HasMaxLength(ShippedTitle.RoleMaxLength)
            .IsRequired();

        builder.Property(x => x.Year)
            .IsRequired();

        builder.HasOne(x => x.CandidateProfile)
            .WithMany(x => x.ShippedTitles)
            .HasForeignKey(x => x.CandidateProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}