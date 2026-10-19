using HireGameDev.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireGameDev.Infrastructure.Persistence.Configurations;

public class StudioCultureTagConfiguration : IEntityTypeConfiguration<StudioCultureTag>
{
    public void Configure(EntityTypeBuilder<StudioCultureTag> builder)
    {
        builder.ToTable("StudioCultureTags");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Tag)
            .HasConversion<int>()
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.StudioProfileId,
            x.Tag
        })
        .IsUnique();

        builder.HasOne(x => x.StudioProfile)
            .WithMany(x => x.CultureTags)
            .HasForeignKey(x => x.StudioProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
