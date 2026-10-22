using HireGameDev.Domain.Entities;
using HireGameDev.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireGameDev.Infrastructure.Persistence.Configurations;

public class JobConfiguration : IEntityTypeConfiguration<Job>
{
    public void Configure(EntityTypeBuilder<Job> builder)
    {
        builder.ToTable("Jobs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .HasMaxLength(Job.TitleMaxLength)
            .IsRequired();

        // Keep enum persistence explicitly integer-backed. Other is a safe migration default for
        // pre-existing jobs; the application still requires a deliberate category on new posts.
        builder.Property(x => x.Category)
            .HasConversion<int>()
            .HasDefaultValue(JobCategory.Other)
            .IsRequired();

        builder.Property(x => x.Type)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.SeniorityLevel)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.LocationMode)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.MinimumAnnualSalary)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.MaximumAnnualSalary)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.City)
            .HasMaxLength(Job.CityMaxLength)
            .IsRequired();

        builder.Property(x => x.Description)
            .IsRequired();

        builder.HasIndex(x => x.Status);

        builder.HasIndex(x => x.SeniorityLevel);

        builder.HasIndex(x => x.City);

        builder.HasIndex(x => x.ExpiresAtUtc);

        builder.HasIndex(x => x.StudioProfileId);

        builder.HasOne(x => x.StudioProfile)
            .WithMany(x => x.Jobs)
            .HasForeignKey(x => x.StudioProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
