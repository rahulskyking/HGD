using HireGameDev.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireGameDev.Infrastructure.Persistence.Configurations;

public class ApplicationStatusHistoryConfiguration
    : IEntityTypeConfiguration<ApplicationStatusHistory>
{
    public void Configure(EntityTypeBuilder<ApplicationStatusHistory> builder)
    {
        builder.ToTable("ApplicationStatusHistories");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new
        {
            x.ApplicationId,
            x.ChangedAtUtc
        });

        builder.HasOne(x => x.Application)
            .WithMany(x => x.StatusHistory)
            .HasForeignKey(x => x.ApplicationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}