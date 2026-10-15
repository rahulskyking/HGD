using HireGameDev.Application.Features.Authentication.SignInWithExternalLogin;
using HireGameDev.Application.Features.CandidateProfiles.GetMyCandidateProfile;
using HireGameDev.Application.Features.CandidateProfiles.GetSkills;
using HireGameDev.Application.Features.CandidateProfiles.RemoveCandidatePhoto;
using HireGameDev.Application.Features.CandidateProfiles.SaveCandidateProfile;
using HireGameDev.Application.Features.CandidateProfiles.UploadCandidatePhoto;
using Microsoft.Extensions.DependencyInjection;

namespace HireGameDev.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Authentication
        services.AddScoped<SignInWithExternalLoginHandler>();

        // Candidate profiles
        services.AddScoped<GetSkillsHandler>();
        services.AddScoped<GetMyCandidateProfileHandler>();
        services.AddScoped<SaveCandidateProfileHandler>();
        services.AddScoped<UploadCandidatePhotoHandler>();
        services.AddScoped<RemoveCandidatePhotoHandler>();

        return services;
    }
}
