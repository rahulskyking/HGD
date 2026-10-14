using HireGameDev.Application.Features.Authentication.Commands.SignInWithExternalLogin;
using Microsoft.Extensions.DependencyInjection;

namespace HireGameDev.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Authentication
        services.AddScoped<SignInWithExternalLoginHandler>();

        return services;
    }
}
