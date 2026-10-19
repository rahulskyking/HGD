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
            .HasMaxLength(StudioProfile.NameMaxLength)
            .IsRequired();

        builder.Property(x => x.LogoUrl)
            .HasMaxLength(StudioProfile.LogoUrlMaxLength);

        builder.Property(x => x.WebsiteUrl)
            .HasMaxLength(StudioProfile.WebsiteUrlMaxLength);

        builder.Property(x => x.City)
            .HasMaxLength(StudioProfile.CityMaxLength)
            .IsRequired();

        builder.Property(x => x.State)
            .HasMaxLength(StudioProfile.StateMaxLength)
            .IsRequired();

        // Enums are stored as their numbers (the convention for every enum in the schema).
        builder.Property(x => x.Size)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Type)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.About)
            .HasMaxLength(StudioProfile.AboutMaxLength);

        // Preserve the existing varchar(30) column width. Domain accepts a valid 15-character
        // GSTIN; retaining extra storage headroom avoids an unnecessary narrowing migration.
        builder.Property(x => x.GstNumber)
            .HasMaxLength(30);

        builder.HasOne<Infrastructure.Identity.ApplicationUser>()
            .WithOne()
            .HasForeignKey<StudioProfile>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // The collections are read-only on the entity; EF loads into the backing lists.
        builder.Navigation(x => x.Engines)
            .HasField("_engines");

        builder.Navigation(x => x.Genres)
            .HasField("_genres");

        builder.Navigation(x => x.CultureTags)
            .HasField("_cultureTags");

        builder.Navigation(x => x.Games)
            .HasField("_games");
    }
}
