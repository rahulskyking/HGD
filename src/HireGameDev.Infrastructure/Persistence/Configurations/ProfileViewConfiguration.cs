using HireGameDev.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireGameDev.Infrastructure.Persistence.Configurations;

public class ProfileViewConfiguration : IEntityTypeConfiguration<ProfileView>
{
    public void Configure(EntityTypeBuilder<ProfileView> builder)
    {
        builder.ToTable("ProfileViews");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new
        {
            x.CandidateProfileId,
            x.ViewedAtUtc
        });

        builder.HasIndex(x => new
        {
            x.ViewerStudioId,
            x.ViewedAtUtc
        });

        builder.HasOne(x => x.CandidateProfile)
            .WithMany()
            .HasForeignKey(x => x.CandidateProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ViewerStudio)
            .WithMany()
            .HasForeignKey(x => x.ViewerStudioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}