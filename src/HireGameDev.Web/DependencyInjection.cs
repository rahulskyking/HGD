using HireGameDev.Web.Authorization;
using HireGameDev.Web.Configuration;

namespace HireGameDev.Web;

/// <summary>
/// Everything the Web layer adds on top of Application and Infrastructure:
/// cookie paths, the external sign-in providers, role policies and Razor Pages conventions.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddWeb(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.LogoutPath = "/Account/Logout";
            options.AccessDeniedPath = "/Account/AccessDenied";
        });

        services.AddExternalSignInProviders(configuration, environment);

        services.AddAuthorizationBuilder()
            .AddPolicy(RolePolicies.Candidate, policy => policy.RequireRole(RolePolicies.Candidate))
            .AddPolicy(RolePolicies.Studio, policy => policy.RequireRole(RolePolicies.Studio));

        services.AddRazorPages(options =>
        {
            // Every page in these folders requires the matching role; new pages are protected by default.
            options.Conventions.AuthorizeFolder("/Candidate", RolePolicies.Candidate);
            options.Conventions.AuthorizeFolder("/Studio", RolePolicies.Studio);
        });

        return services;
    }

    private static void AddExternalSignInProviders(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        var authentication = services.AddAuthentication();

        // Google sign-in is part of the MVP: required outside Development, optional locally
        // so the app can run without OAuth credentials.
        var google = configuration.GetSection("Authentication:Google").Get<OAuthProviderSettings>();

        if (google is { IsConfigured: true })
        {
            authentication.AddGoogle(options =>
            {
                options.ClientId = google.ClientId!;
                options.ClientSecret = google.ClientSecret!;
            });
        }
        else if (!environment.IsDevelopment())
        {
            throw new InvalidOperationException(
                "Google sign-in is not configured. Set 'Authentication:Google:ClientId' and " +
                "'Authentication:Google:ClientSecret'.");
        }

        // GitHub sign-in is optional in every environment.
        var gitHub = configuration.GetSection("Authentication:GitHub").Get<OAuthProviderSettings>();

        if (gitHub is { IsConfigured: true })
        {
            authentication.AddGitHub(options =>
            {
                options.ClientId = gitHub.ClientId!;
                options.ClientSecret = gitHub.ClientSecret!;
                options.Scope.Add("user:email");
            });
        }
    }
}
