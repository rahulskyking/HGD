using HireGameDev.Domain.Entities;
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
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.MinimumAnnualSalary)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.MaximumAnnualSalary)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.City)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Description)
            .IsRequired();

        builder.HasIndex(x => x.Status);

        builder.HasIndex(x => x.SeniorityLevel);

        builder.HasIndex(x => x.City);

        builder.HasIndex(x => x.ExpiresAtUtc);

        builder.HasOne(x => x.StudioProfile)
            .WithMany(x => x.Jobs)
            .HasForeignKey(x => x.StudioProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}