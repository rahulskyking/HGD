using AspNet.Security.OAuth.GitHub;

namespace HireGameDev.Web.Configuration;

public static class AuthenticationConfiguration
{
    public static IServiceCollection AddAuthenticationProviders(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddAuthentication()
            .AddGoogle(options =>
            {
                options.ClientId =
                    configuration["Authentication:Google:ClientId"]
                    ?? throw new InvalidOperationException(
                        "Google OAuth ClientId is not configured.");

                options.ClientSecret =
                    configuration["Authentication:Google:ClientSecret"]
                    ?? throw new InvalidOperationException(
                        "Google OAuth ClientSecret is not configured.");
            })
            .AddGitHub(options =>
            {
                options.ClientId =
                    configuration["Authentication:GitHub:ClientId"]
                    ?? throw new InvalidOperationException(
                        "GitHub OAuth ClientId is not configured.");

                options.ClientSecret =
                    configuration["Authentication:GitHub:ClientSecret"]
                    ?? throw new InvalidOperationException(
                        "GitHub OAuth ClientSecret is not configured.");

                options.Scope.Add("user:email");
            });

        return services;
    }
}