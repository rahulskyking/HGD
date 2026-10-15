using System.ComponentModel.DataAnnotations;
using HireGameDev.Application.Features.CandidateProfiles.GetMyCandidateProfile;
using HireGameDev.Application.Features.CandidateProfiles.GetSkills;
using HireGameDev.Application.Features.CandidateProfiles.RemoveCandidatePhoto;
using HireGameDev.Application.Features.CandidateProfiles.SaveCandidateProfile;
using HireGameDev.Application.Features.CandidateProfiles.UploadCandidatePhoto;
using HireGameDev.Domain.Entities;
using HireGameDev.Domain.Enums;
using HireGameDev.Web.Authorization;
using HireGameDev.Web.Display;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HireGameDev.Web.Pages.Candidate.Profile;

/// <summary>
/// Create or edit the whole profile in one form. The photo has its own small forms
/// (upload / remove) that post to separate handlers and redirect back here.
/// </summary>
[RequestFormLimits(MultipartBodyLengthLimit = UploadCandidatePhotoCommand.MaxBytes + 64 * 1024)]
public class EditModel(
    GetMyCandidateProfileHandler getProfile,
    GetSkillsHandler getSkills,
    SaveCandidateProfileHandler saveProfile,
    UploadCandidatePhotoHandler uploadPhoto,
    RemoveCandidatePhotoHandler removePhoto) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();

    [BindProperty]
    public IFormFile? Photo { get; set; }

    [TempData]
    public string? StatusMessage { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    /// <summary>True until the first save; the photo card needs a saved profile to attach to.</summary>
    public bool IsNew { get; private set; } = true;

    public string? PhotoUrl { get; private set; }

    public IReadOnlyList<SkillDto> Skills { get; private set; } = [];

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        var profile = await LoadAsync(cancellationToken);

        if (profile is not null)
        {
            Input = InputModel.From(profile);
        }
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await LoadAsync(cancellationToken);
            return Page();
        }

        var result = await saveProfile.HandleAsync(Input.ToCommand(User.GetUserId()), cancellationToken);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            await LoadAsync(cancellationToken);
            return Page();
        }

        StatusMessage = "Profile saved.";
        return RedirectToPage("./Index");
    }

    public async Task<IActionResult> OnPostUploadPhotoAsync(CancellationToken cancellationToken)
    {
        if (Photo is null)
        {
            ErrorMessage = "Choose a photo to upload.";
            return RedirectToPage();
        }

        await using var content = Photo.OpenReadStream();

        var result = await uploadPhoto.HandleAsync(
            new UploadCandidatePhotoCommand(User.GetUserId(), content, Photo.Length), cancellationToken);

        if (result.Succeeded)
        {
            StatusMessage = "Photo updated.";
        }
        else
        {
            ErrorMessage = string.Join(" ", result.Errors);
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRemovePhotoAsync(CancellationToken cancellationToken)
    {
        await removePhoto.HandleAsync(new RemoveCandidatePhotoCommand(User.GetUserId()), cancellationToken);

        StatusMessage = "Photo removed.";
        return RedirectToPage();
    }

    /// <summary>Loads what every render needs (catalogue + photo state) and returns the profile, if any.</summary>
    private async Task<CandidateProfileDto?> LoadAsync(CancellationToken cancellationToken)
    {
        Skills = await getSkills.HandleAsync(new GetSkillsQuery(), cancellationToken);

        var profile = await getProfile.HandleAsync(new GetMyCandidateProfileQuery(User.GetUserId()), cancellationToken);

        IsNew = profile is null;
        PhotoUrl = profile?.PhotoUrl;

        return profile;
    }

    public class InputModel
    {
        [Required]
        [StringLength(CandidateProfile.FullNameMaxLength)]
        [Display(Name = "Full name")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [StringLength(CandidateProfile.JobTitleMaxLength)]
        [Display(Name = "Job title")]
        public string JobTitle { get; set; } = string.Empty;

        [Required]
        [StringLength(CandidateProfile.CityMaxLength)]
        public string City { get; set; } = string.Empty;

        [Phone]
        [StringLength(CandidateProfile.PhoneMaxLength)]
        [Display(Name = "Phone (optional)")]
        public string? Phone { get; set; }

        [Required]
        [Display(Name = "Seniority")]
        public SeniorityLevel? SeniorityLevel { get; set; }

        [Required]
        [Range(0, CandidateProfile.MaximumYearsOfExperience)]
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

        [StringLength(CandidateProfile.AboutMaxLength)]
        [Display(Name = "About (optional)")]
        public string? About { get; set; }

        [MinLength(CandidateProfile.MinimumSkillCount, ErrorMessage = "Pick at least {1} skills.")]
        [MaxLength(CandidateProfile.MaximumSkillCount, ErrorMessage = "Pick at most {1} skills.")]
        [Display(Name = "Skills")]
        public List<Guid> SkillIds { get; set; } = [];

        [MaxLength(CandidateProfile.MaximumShippedTitles, ErrorMessage = "Add at most {1} shipped titles.")]
        public List<ShippedTitleInput> ShippedTitles { get; set; } = [];

        [MaxLength(CandidateProfile.MaximumPortfolioLinks, ErrorMessage = "Add at most {1} portfolio links.")]
        public List<PortfolioLinkInput> PortfolioLinks { get; set; } = [];

        public static InputModel From(CandidateProfileDto profile) => new()
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
                .Select(x => new ShippedTitleInput { GameName = x.GameName, StudioName = x.StudioName, Role = x.Role, Year = x.Year })
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
}
