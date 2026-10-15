using HireGameDev.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireGameDev.Infrastructure.Persistence.Configurations;

public class PortfolioLinkConfiguration : IEntityTypeConfiguration<PortfolioLink>
{
    public void Configure(EntityTypeBuilder<PortfolioLink> builder)
    {
        builder.ToTable("PortfolioLinks");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Url)
            .HasMaxLength(PortfolioLink.UrlMaxLength)
            .IsRequired();

        builder.Property(x => x.Label)
            .HasMaxLength(PortfolioLink.LabelMaxLength);

        builder.HasOne(x => x.CandidateProfile)
            .WithMany(x => x.PortfolioLinks)
            .HasForeignKey(x => x.CandidateProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
