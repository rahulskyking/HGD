using HireGameDev.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DomainApplication = HireGameDev.Domain.Entities.Application;


namespace HireGameDev.Infrastructure.Persistence.Configurations;

public class ApplicationConfiguration
    : IEntityTypeConfiguration<DomainApplication>
{
    public void Configure(EntityTypeBuilder<DomainApplication> builder)
    {
        builder.ToTable("Applications");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.MatchScore)
            .HasPrecision(5, 2);

        builder.HasIndex(x => new
        {
            x.CandidateProfileId,
            x.JobId
        })
        .IsUnique();

        builder.HasIndex(x => x.Status);

        builder.HasIndex(x => x.LastActionAtUtc);

        builder.HasOne(x => x.CandidateProfile)
            .WithMany()
            .HasForeignKey(x => x.CandidateProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Job)
            .WithMany(x => x.Applications)
            .HasForeignKey(x => x.JobId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}