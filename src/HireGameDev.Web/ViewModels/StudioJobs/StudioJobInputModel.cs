using System.ComponentModel.DataAnnotations;
using HireGameDev.Application.Features.Jobs.GetStudioJob;
using HireGameDev.Application.Features.Jobs.SaveStudioJob;
using HireGameDev.Domain.Enums;
using HireGameDev.Web.Display;

namespace HireGameDev.Web.ViewModels.StudioJobs;

public sealed class StudioJobInputModel : IValidatableObject
{
    [Required]
    [StringLength(200)]
    [Display(Name = "Job title")]
    public string Title { get; set; } = string.Empty;

    [Required]
    [EnumDataType(typeof(JobCategory))]
    [Display(Name = "Category")]
    public JobCategory? Category { get; set; }

    [Required]
    [EnumDataType(typeof(JobType))]
    [Display(Name = "Job type")]
    public JobType? Type { get; set; }

    [Required]
    [EnumDataType(typeof(SeniorityLevel))]
    [Display(Name = "Seniority")]
    public SeniorityLevel? SeniorityLevel { get; set; }

    [Required]
    [Range(0.1, 500)]
    [Display(Name = "Minimum salary (LPA)")]
    public decimal? MinimumSalaryLpa { get; set; }

    [Required]
    [Range(0.1, 500)]
    [Display(Name = "Maximum salary (LPA)")]
    public decimal? MaximumSalaryLpa { get; set; }

    [Required]
    [EnumDataType(typeof(LocationMode))]
    [Display(Name = "Work location")]
    public LocationMode? LocationMode { get; set; }

    [Required]
    [StringLength(100)]
    [Display(Name = "City")]
    public string City { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Job description")]
    public string Description { get; set; } = string.Empty;

    [Display(Name = "Required skills")]
    public List<Guid> RequiredSkillIds { get; set; } = [];

    [Display(Name = "Nice-to-have skills")]
    public List<Guid> NiceToHaveSkillIds { get; set; } = [];

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (MinimumSalaryLpa is { } minimum
            && MaximumSalaryLpa is { } maximum
            && maximum < minimum)
        {
            yield return new ValidationResult(
                "Maximum salary must be at least the minimum salary.",
                [nameof(MaximumSalaryLpa)]);
        }

        var allSkillIds = RequiredSkillIds.Concat(NiceToHaveSkillIds).ToList();

        if (allSkillIds.Count != allSkillIds.Distinct().Count())
        {
            yield return new ValidationResult(
                "A skill cannot be selected more than once or in both lists.",
                [nameof(RequiredSkillIds), nameof(NiceToHaveSkillIds)]);
        }
    }

    public SaveStudioJobCommand ToCommand(Guid userId, Guid? jobId) => new(
        userId,
        jobId,
        Title,
        Category!.Value,
        Type!.Value,
        SeniorityLevel!.Value,
        Salary.ToRupees(MinimumSalaryLpa!.Value),
        Salary.ToRupees(MaximumSalaryLpa!.Value),
        LocationMode!.Value,
        City,
        Description,
        RequiredSkillIds,
        NiceToHaveSkillIds);

    public static StudioJobInputModel From(StudioJobDto job) => new()
    {
        Title = job.Title,
        Category = job.Category,
        Type = job.Type,
        SeniorityLevel = job.SeniorityLevel,
        MinimumSalaryLpa = Salary.ToLpa(job.MinimumAnnualSalary),
        MaximumSalaryLpa = Salary.ToLpa(job.MaximumAnnualSalary),
        LocationMode = job.LocationMode,
        City = job.City,
        Description = job.Description,
        RequiredSkillIds = job.Skills.Where(skill => skill.IsRequired).Select(skill => skill.SkillId).ToList(),
        NiceToHaveSkillIds = job.Skills.Where(skill => !skill.IsRequired).Select(skill => skill.SkillId).ToList(),
    };
}
