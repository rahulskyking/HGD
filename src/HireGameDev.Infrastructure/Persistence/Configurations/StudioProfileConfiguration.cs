using HireGameDev.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireGameDev.Infrastructure.Persistence.Configurations;

public class StudioProfileConfiguration : IEntityTypeConfiguration<StudioProfile>
{
    public void Configure(EntityTypeBuilder<StudioProfile> builder)
    {
        builder.ToTable("StudioProfiles");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.HasIndex(x => x.UserId)
            .IsUnique();

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.LogoUrl)
            .HasMaxLength(500);

        builder.Property(x => x.WebsiteUrl)
            .HasMaxLength(500);

        builder.Property(x => x.City)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.State)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Size)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Type)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.GstNumber)
            .HasMaxLength(30);

        builder.HasOne<Infrastructure.Identity.ApplicationUser>()
            .WithOne()
            .HasForeignKey<StudioProfile>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}