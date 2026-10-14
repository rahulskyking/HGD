using HireGameDev.Application.Features.Authentication.Commands.CompleteExternalRegistration;
using HireGameDev.Application.Features.Authentication.Commands.RegisterUser;
using HireGameDev.Application.Features.Authentication.Commands.ResendConfirmationEmail;
using HireGameDev.Application.Features.Authentication.Commands.SignInWithExternalLogin;
using Microsoft.Extensions.DependencyInjection;

namespace HireGameDev.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Authentication
        services.AddScoped<RegisterUserHandler>();
        services.AddScoped<ResendConfirmationEmailHandler>();
        services.AddScoped<SignInWithExternalLoginHandler>();
        services.AddScoped<CompleteExternalRegistrationHandler>();

        return services;
    }
}
