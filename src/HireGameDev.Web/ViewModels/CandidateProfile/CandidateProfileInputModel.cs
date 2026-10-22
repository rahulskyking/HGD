using System.ComponentModel.DataAnnotations;
using HireGameDev.Application.Features.CandidateProfiles.GetMyCandidateProfile;
using HireGameDev.Application.Features.CandidateProfiles.SaveCandidateProfile;
using CandidateProfileEntity = global::HireGameDev.Domain.Entities.CandidateProfile;
using HireGameDev.Domain.Enums;
using HireGameDev.Web.Display;

namespace HireGameDev.Web.ViewModels.CandidateProfile;

public sealed class CandidateProfileInputModel
{
    [Required]
    [StringLength(CandidateProfileEntity.FullNameMaxLength)]
    [Display(Name = "Full name")]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [StringLength(CandidateProfileEntity.JobTitleMaxLength)]
    [Display(Name = "Job title")]
    public string JobTitle { get; set; } = string.Empty;

    [Required]
    [StringLength(CandidateProfileEntity.CityMaxLength)]
    public string City { get; set; } = string.Empty;

    [Phone]
    [StringLength(CandidateProfileEntity.PhoneMaxLength)]
    [Display(Name = "Phone (optional)")]
    public string? Phone { get; set; }

    [Required]
    [Display(Name = "Seniority")]
    public SeniorityLevel? SeniorityLevel { get; set; }

    [Required]
    [Range(0, CandidateProfileEntity.MaximumYearsOfExperience)]
    [Display(Name = "Years of experience")]
    public decimal? YearsOfExperience { get; set; }

    [Range(0, 500)]
    [Display(Name = "Current salary (LPA, optional)")]
    public decimal? CurrentSalaryLpa { get; set; }

    [Required]
    [Range(0.1, 500)]
    [Display(Name = "Expected salary (LPA)")]
    public decimal? ExpectedSalaryLpa { get; set; }

    [Required]
    [Display(Name = "Notice period")]
    public NoticePeriod? NoticePeriod { get; set; }

    [Required]
    [Display(Name = "Job search status")]
    public OpenToWorkStatus? OpenToWorkStatus { get; set; }

    [Display(Name = "Open to remote work")]
    public bool OpenToRemote { get; set; }

    [StringLength(CandidateProfileEntity.AboutMaxLength)]
    [Display(Name = "About (optional)")]
    public string? About { get; set; }

    [MinLength(CandidateProfileEntity.MinimumSkillCount, ErrorMessage = "Pick at least {1} skills.")]
    [MaxLength(CandidateProfileEntity.MaximumSkillCount, ErrorMessage = "Pick at most {1} skills.")]
    [Display(Name = "Skills")]
    public List<Guid> SkillIds { get; set; } = [];

    [MaxLength(CandidateProfileEntity.MaximumShippedTitles, ErrorMessage = "Add at most {1} shipped titles.")]
    public List<ShippedTitleInput> ShippedTitles { get; set; } = [];

    [MaxLength(CandidateProfileEntity.MaximumPortfolioLinks, ErrorMessage = "Add at most {1} portfolio links.")]
    public List<PortfolioLinkInput> PortfolioLinks { get; set; } = [];

    public static CandidateProfileInputModel From(CandidateProfileDto profile) => new()
    {
        FullName = profile.FullName,
        JobTitle = profile.JobTitle,
        City = profile.City,
        Phone = profile.Phone,
        SeniorityLevel = profile.SeniorityLevel,
        YearsOfExperience = profile.YearsOfExperience,
        CurrentSalaryLpa = profile.CurrentAnnualSalary is { } current ? Salary.ToLpa(current) : null,
        ExpectedSalaryLpa = Salary.ToLpa(profile.ExpectedAnnualSalary),
        NoticePeriod = profile.NoticePeriod,
        OpenToWorkStatus = profile.OpenToWorkStatus,
        OpenToRemote = profile.OpenToRemote,
        About = profile.About,
        SkillIds = profile.Skills.Select(x => x.SkillId).ToList(),
        ShippedTitles = profile.ShippedTitles
            .Select(x => new ShippedTitleInput
            {
                GameName = x.GameName,
                StudioName = x.StudioName,
                Role = x.Role,
                Year = x.Year,
            })
            .ToList(),
        PortfolioLinks = profile.PortfolioLinks
            .Select(x => new PortfolioLinkInput { Url = x.Url, Label = x.Label })
            .ToList(),
    };

    /// <summary>Only called after validation, so the required values are present.</summary>
    public SaveCandidateProfileCommand ToCommand(Guid userId) => new(
        userId,
        FullName,
        Phone,
        City,
        JobTitle,
        SeniorityLevel!.Value,
        YearsOfExperience!.Value,
        CurrentSalaryLpa is { } current ? Salary.ToRupees(current) : null,
        Salary.ToRupees(ExpectedSalaryLpa!.Value),
        NoticePeriod!.Value,
        OpenToRemote,
        OpenToWorkStatus!.Value,
        About,
        SkillIds,
        ShippedTitles.Select(x => x.ToDetails()).ToList(),
        PortfolioLinks.Select(x => x.ToDetails()).ToList());
}
