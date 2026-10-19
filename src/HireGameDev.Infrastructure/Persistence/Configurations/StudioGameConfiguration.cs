using HireGameDev.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireGameDev.Infrastructure.Persistence.Configurations;

public class StudioGameConfiguration : IEntityTypeConfiguration<StudioGame>
{
    public void Configure(EntityTypeBuilder<StudioGame> builder)
    {
        builder.ToTable("StudioGames");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(StudioGame.NameMaxLength)
            .IsRequired();

        builder.Property(x => x.Year)
            .IsRequired();

        builder.Property(x => x.Url)
            .HasMaxLength(StudioGame.UrlMaxLength);

        builder.HasOne(x => x.StudioProfile)
            .WithMany(x => x.Games)
            .HasForeignKey(x => x.StudioProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
