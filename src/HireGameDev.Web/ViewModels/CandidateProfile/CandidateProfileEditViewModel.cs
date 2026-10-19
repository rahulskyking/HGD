using HireGameDev.Application.Features.Skills.GetSkills;
using Microsoft.AspNetCore.Http;

namespace HireGameDev.Web.ViewModels.CandidateProfile;

public sealed class CandidateProfileEditViewModel
{
    public CandidateProfileInputModel Input { get; set; } = new();

    public IFormFile? Photo { get; set; }

    public bool IsNew { get; set; } = true;

    public string? PhotoUrl { get; set; }

    public IReadOnlyList<SkillDto> Skills { get; set; } = [];

    public string? StatusMessage { get; set; }

    public string? ErrorMessage { get; set; }
}
