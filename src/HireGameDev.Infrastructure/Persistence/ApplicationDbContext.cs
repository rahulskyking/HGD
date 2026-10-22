using HireGameDev.Application.Persistence;
using HireGameDev.Domain.Entities;
using HireGameDev.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DomainApplication = HireGameDev.Domain.Entities.Application;

namespace HireGameDev.Infrastructure.Persistence;

public class ApplicationDbContext
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>, IApplicationUnitOfWork
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<CandidateProfile> CandidateProfiles => Set<CandidateProfile>();

    public DbSet<StudioProfile> StudioProfiles => Set<StudioProfile>();

    public DbSet<StudioEngine> StudioEngines => Set<StudioEngine>();

    public DbSet<StudioGenre> StudioGenres => Set<StudioGenre>();

    public DbSet<StudioCultureTag> StudioCultureTags => Set<StudioCultureTag>();

    public DbSet<StudioGame> StudioGames => Set<StudioGame>();

    public DbSet<Skill> Skills => Set<Skill>();

    public DbSet<CandidateSkill> CandidateSkills => Set<CandidateSkill>();

    public DbSet<ShippedTitle> ShippedTitles => Set<ShippedTitle>();

    public DbSet<PortfolioLink> PortfolioLinks => Set<PortfolioLink>();

    public DbSet<Job> Jobs => Set<Job>();

    public DbSet<JobSkill> JobSkills => Set<JobSkill>();

    public DbSet<DomainApplication> Applications => Set<DomainApplication>();

    public DbSet<ApplicationStatusHistory> ApplicationStatusHistories =>
        Set<ApplicationStatusHistory>();

    public DbSet<Notification> Notifications => Set<Notification>();

    public DbSet<ProfileView> ProfileViews => Set<ProfileView>();

    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);
    }
}