using HireGameDev.Application.Features.Skills.GetSkills;
using HireGameDev.Domain.Enums;

namespace HireGameDev.Web.ViewModels.StudioJobs;

public sealed class StudioJobEditViewModel
{
    public Guid? JobId { get; set; }

    public bool IsNew { get; set; } = true;

    public JobStatus? Status { get; set; }

    public DateTime? ExpiresAtUtc { get; set; }

    public StudioJobInputModel Input { get; set; } = new();

    public IReadOnlyList<SkillDto> Skills { get; set; } = [];

    public string? StatusMessage { get; set; }

    public string? ErrorMessage { get; set; }
}
