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
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.PhotoUrl)
            .HasMaxLength(500);

        builder.Property(x => x.Phone)
            .HasMaxLength(20);

        builder.Property(x => x.City)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.JobTitle)
            .HasMaxLength(150)
            .IsRequired();

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
    }
}