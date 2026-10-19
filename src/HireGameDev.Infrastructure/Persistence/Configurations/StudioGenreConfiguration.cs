using HireGameDev.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireGameDev.Infrastructure.Persistence.Configurations;

public class StudioGenreConfiguration : IEntityTypeConfiguration<StudioGenre>
{
    public void Configure(EntityTypeBuilder<StudioGenre> builder)
    {
        builder.ToTable("StudioGenres");

        builder.HasKey(x => x.Id);

        // GameGenre is stored as an integer, consistently with the other Domain enums.
        builder.Property(x => x.Genre)
            .HasConversion<int>()
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.StudioProfileId,
            x.Genre
        })
        .IsUnique();

        builder.HasOne(x => x.StudioProfile)
            .WithMany(x => x.Genres)
            .HasForeignKey(x => x.StudioProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
