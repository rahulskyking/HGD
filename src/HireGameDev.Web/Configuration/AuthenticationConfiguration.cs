namespace HireGameDev.Web.Configuration;

public static class AuthenticationConfiguration
{
    public static IServiceCollection AddAuthenticationProviders(
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

        return services;
    }
}
