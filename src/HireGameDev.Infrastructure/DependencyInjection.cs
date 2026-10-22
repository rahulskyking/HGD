using HireGameDev.Application.Identity;
using HireGameDev.Application.Persistence;
using HireGameDev.Application.Storage;
using HireGameDev.Infrastructure.Identity;
using HireGameDev.Infrastructure.Persistence;
using HireGameDev.Infrastructure.Storage;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HireGameDev.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' was not found.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Application-facing persistence contracts are implemented here; EF stays in Infrastructure.
        services.AddScoped<IApplicationUnitOfWork>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<ICandidateProfileRepository, CandidateProfileRepository>();
        services.AddScoped<IStudioProfileRepository, StudioProfileRepository>();
        services.AddScoped<ISkillRepository, SkillRepository>();

        services
            .AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
            {
                options.User.RequireUniqueEmail = true;

                // Accounts are only created from a provider-verified email (no passwords, ADR-013).
                // Keep the gate anyway so an unverified account can never sign in.
                options.SignIn.RequireConfirmedEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>();

        services.AddScoped<IIdentityService, IdentityService>();

        services.Configure<FileStorageSettings>(
            configuration.GetSection(FileStorageSettings.SectionName));
        services.AddSingleton<IFileStorage, LocalFileStorage>();

        return services;
    }
}
