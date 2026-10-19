using HireGameDev.Application.Features.Skills.GetSkills;
using Microsoft.AspNetCore.Http;

namespace HireGameDev.Web.ViewModels.StudioProfile;

public sealed class StudioProfileEditViewModel
{
    public StudioProfileInputModel Input { get; set; } = new();

    public IFormFile? Logo { get; set; }

    public bool IsNew { get; set; } = true;

    public string? LogoUrl { get; set; }

    public IReadOnlyList<SkillDto> Engines { get; set; } = [];

    public string? StatusMessage { get; set; }

    public string? ErrorMessage { get; set; }
}
