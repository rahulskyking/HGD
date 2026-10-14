using System.Net.Http.Headers;
using HireGameDev.Application.Email;
using HireGameDev.Application.Identity;
using HireGameDev.Infrastructure.Email;
using HireGameDev.Infrastructure.Identity;
using HireGameDev.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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

        services
            .AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
            {
                options.User.RequireUniqueEmail = true;

                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;

                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.AllowedForNewUsers = true;

                // Password accounts must confirm their email before they can sign in.
                options.SignIn.RequireConfirmedEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IIdentityService, IdentityService>();

        services.AddEmailSender(configuration);

        return services;
    }

    private static IServiceCollection AddEmailSender(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var emailSection = configuration.GetSection(EmailOptions.SectionName);
        var emailOptions = emailSection.Get<EmailOptions>() ?? new EmailOptions();

        if (string.Equals(emailOptions.Provider, EmailOptions.LogProvider, StringComparison.OrdinalIgnoreCase))
        {
            services.AddSingleton<IEmailSender, LogEmailSender>();

            return services;
        }

        if (string.Equals(emailOptions.Provider, EmailOptions.ResendProvider, StringComparison.OrdinalIgnoreCase))
        {
            if (string.IsNullOrWhiteSpace(emailOptions.Resend.ApiKey)
                || string.IsNullOrWhiteSpace(emailOptions.Resend.FromAddress))
            {
                throw new InvalidOperationException(
                    "Email provider 'Resend' requires 'Email:Resend:ApiKey' and 'Email:Resend:FromAddress'. " +
                    "For local development without Resend, set 'Email:Provider' to 'Log'.");
            }

            services.Configure<ResendOptions>(emailSection.GetSection(nameof(EmailOptions.Resend)));

            services.AddHttpClient<IEmailSender, ResendEmailSender>((serviceProvider, client) =>
            {
                var resend = serviceProvider.GetRequiredService<IOptions<ResendOptions>>().Value;

                client.BaseAddress = new Uri("https://api.resend.com/");
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", resend.ApiKey);
            });

            return services;
        }

        throw new InvalidOperationException(
            $"Unknown email provider '{emailOptions.Provider}'. Supported values: " +
            $"'{EmailOptions.ResendProvider}', '{EmailOptions.LogProvider}'.");
    }
}
