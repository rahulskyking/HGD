using HireGameDev.Application.Features.Authentication.SignInWithExternalLogin;
using HireGameDev.Application.Features.CandidateProfiles.GetMyCandidateProfile;
using HireGameDev.Application.Features.CandidateProfiles.RemoveCandidatePhoto;
using HireGameDev.Application.Features.CandidateProfiles.SaveCandidateProfile;
using HireGameDev.Application.Features.CandidateProfiles.UploadCandidatePhoto;
using HireGameDev.Application.Features.Skills.GetSkills;
using HireGameDev.Application.Features.Jobs.ChangeStudioJobStatus;
using HireGameDev.Application.Features.Jobs.GetStudioJob;
using HireGameDev.Application.Features.Jobs.GetStudioJobs;
using HireGameDev.Application.Features.Jobs.SaveStudioJob;
using HireGameDev.Application.Features.StudioProfiles.GetMyStudioProfile;
using HireGameDev.Application.Features.StudioProfiles.RemoveStudioLogo;
using HireGameDev.Application.Features.StudioProfiles.SaveStudioProfile;
using HireGameDev.Application.Features.StudioProfiles.UploadStudioLogo;
using Microsoft.Extensions.DependencyInjection;

namespace HireGameDev.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Authentication
        services.AddScoped<SignInWithExternalLoginHandler>();

        // Skills master list
        services.AddScoped<GetSkillsHandler>();

        // Studio job posts
        services.AddScoped<GetStudioJobsHandler>();
        services.AddScoped<GetStudioJobHandler>();
        services.AddScoped<SaveStudioJobHandler>();
        services.AddScoped<ChangeStudioJobStatusHandler>();

        // Candidate profiles
        services.AddScoped<GetMyCandidateProfileHandler>();
        services.AddScoped<SaveCandidateProfileHandler>();
        services.AddScoped<UploadCandidatePhotoHandler>();
        services.AddScoped<RemoveCandidatePhotoHandler>();

        // Studio profiles
        services.AddScoped<GetMyStudioProfileHandler>();
        services.AddScoped<SaveStudioProfileHandler>();
        services.AddScoped<UploadStudioLogoHandler>();
        services.AddScoped<RemoveStudioLogoHandler>();

        return services;
    }
}
