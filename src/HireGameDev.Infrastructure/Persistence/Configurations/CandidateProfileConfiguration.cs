using HireGameDev.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireGameDev.Infrastructure.Persistence.Configurations;

public class CandidateProfileConfiguration : IEntityTypeConfiguration<CandidateProfile>
{
    public void Configure(EntityTypeBuilder<CandidateProfile> builder)
    {
        builder.ToTable("CandidateProfiles");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.HasIndex(x => x.UserId)
            .IsUnique();

        builder.Property(x => x.FullName)
            .HasMaxLength(CandidateProfile.FullNameMaxLength)
            .IsRequired();

        builder.Property(x => x.PhotoUrl)
            .HasMaxLength(CandidateProfile.PhotoUrlMaxLength);

        builder.Property(x => x.Phone)
            .HasMaxLength(CandidateProfile.PhoneMaxLength);

        builder.Property(x => x.City)
            .HasMaxLength(CandidateProfile.CityMaxLength)
            .IsRequired();

        builder.Property(x => x.JobTitle)
            .HasMaxLength(CandidateProfile.JobTitleMaxLength)
            .IsRequired();

        builder.Property(x => x.About)
            .HasMaxLength(CandidateProfile.AboutMaxLength);

        builder.Property(x => x.YearsOfExperience)
            .HasPrecision(5, 2);

        builder.Property(x => x.CurrentAnnualSalary)
            .HasPrecision(18, 2);

        builder.Property(x => x.ExpectedAnnualSalary)
            .HasPrecision(18, 2);

        builder.HasOne<Infrastructure.Identity.ApplicationUser>()
            .WithOne()
            .HasForeignKey<CandidateProfile>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // The collections are read-only on the entity; EF loads into the backing lists.
        builder.Navigation(x => x.CandidateSkills)
            .HasField("_candidateSkills");

        builder.Navigation(x => x.ShippedTitles)
            .HasField("_shippedTitles");

        builder.Navigation(x => x.PortfolioLinks)
            .HasField("_portfolioLinks");
    }
}