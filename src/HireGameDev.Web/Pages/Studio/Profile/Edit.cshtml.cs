using System.ComponentModel.DataAnnotations;
using HireGameDev.Application.Features.Skills.GetSkills;
using HireGameDev.Application.Features.StudioProfiles.GetMyStudioProfile;
using HireGameDev.Application.Features.StudioProfiles.RemoveStudioLogo;
using HireGameDev.Application.Features.StudioProfiles.SaveStudioProfile;
using HireGameDev.Application.Features.StudioProfiles.UploadStudioLogo;
using HireGameDev.Domain;
using HireGameDev.Domain.Entities;
using HireGameDev.Domain.Enums;
using HireGameDev.Web.Authorization;
using HireGameDev.Web.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HireGameDev.Web.Pages.Studio.Profile;

/// <summary>
/// Create or edit the studio's profile in one form. The logo uses its own small form and
/// requires a saved profile; uploads are checked in the Application handler.
/// </summary>
[RequestFormLimits(MultipartBodyLengthLimit = UploadStudioLogoCommand.MaxBytes + 64 * 1024)]
public class EditModel(
    GetMyStudioProfileHandler getProfile,
    GetSkillsHandler getSkills,
    SaveStudioProfileHandler saveProfile,
    UploadStudioLogoHandler uploadLogo,
    RemoveStudioLogoHandler removeLogo) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();

    [BindProperty]
    public IFormFile? Logo { get; set; }

    [TempData]
    public string? StatusMessage { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public bool IsNew { get; private set; } = true;

    public string? LogoUrl { get; private set; }

    public IReadOnlyList<SkillDto> Engines { get; private set; } = [];

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

        StatusMessage = "Studio profile saved.";
        return RedirectToPage("./Index");
    }

    public async Task<IActionResult> OnPostUploadLogoAsync(CancellationToken cancellationToken)
    {
        if (Logo is null)
        {
            ErrorMessage = "Choose a logo to upload.";
            return RedirectToPage();
        }

        await using var content = Logo.OpenReadStream();

        var result = await uploadLogo.HandleAsync(
            new UploadStudioLogoCommand(User.GetUserId(), content, Logo.Length), cancellationToken);

        if (result.Succeeded)
        {
            StatusMessage = "Logo updated.";
        }
        else
        {
            ErrorMessage = string.Join(" ", result.Errors);
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRemoveLogoAsync(CancellationToken cancellationToken)
    {
        await removeLogo.HandleAsync(new RemoveStudioLogoCommand(User.GetUserId()), cancellationToken);

        StatusMessage = "Logo removed.";
        return RedirectToPage();
    }

    private async Task<StudioProfileDto?> LoadAsync(CancellationToken cancellationToken)
    {
        var skills = await getSkills.HandleAsync(new GetSkillsQuery(), cancellationToken);
        Engines = skills.Where(x => x.Category == SkillCategory.Engines).ToList();

        var profile = await getProfile.HandleAsync(
            new GetMyStudioProfileQuery(User.GetUserId()), cancellationToken);

        IsNew = profile is null;
        LogoUrl = profile?.LogoUrl;

        return profile;
    }

    public class InputModel : IValidatableObject
    {
        [Required]
        [StringLength(StudioProfile.NameMaxLength)]
        [Display(Name = "Studio name")]
        public string Name { get; set; } = string.Empty;

        [StringLength(StudioProfile.WebsiteUrlMaxLength)]
        [Display(Name = "Website (optional)")]
        public string? WebsiteUrl { get; set; }

        [Required]
        [StringLength(StudioProfile.CityMaxLength)]
        [Display(Name = "City")]
        public string City { get; set; } = string.Empty;

        [Required]
        [EnumDataType(typeof(StudioSize))]
        [Display(Name = "Studio size")]
        public StudioSize? Size { get; set; }

        [Required]
        [EnumDataType(typeof(StudioType))]
        [Display(Name = "Studio type")]
        public StudioType? Type { get; set; }

        [StringLength(StudioProfile.AboutMaxLength)]
        [Display(Name = "About your studio (optional)")]
        public string? About { get; set; }

        [StringLength(StudioProfile.GstNumberLength)]
        [Display(Name = "GSTIN (optional)")]
        public string? GstNumber { get; set; }

        [MaxLength(StudioProfile.MaximumEngines, ErrorMessage = "Pick at most {1} engines.")]
        [Display(Name = "Engines")]
        public List<Guid> EngineIds { get; set; } = [];

        [MaxLength(StudioProfile.MaximumGenres, ErrorMessage = "Pick at most {1} genres.")]
        [Display(Name = "Genres")]
        public List<GameGenre> Genres { get; set; } = [];

        [MaxLength(StudioProfile.MaximumCultureTags, ErrorMessage = "Pick at most {1} culture tags.")]
        [Display(Name = "Culture and benefits")]
        public List<CultureTag> CultureTags { get; set; } = [];

        [MaxLength(StudioProfile.MaximumGames, ErrorMessage = "Add at most {1} released games.")]
        [Display(Name = "Released games")]
        public List<StudioGameInput> Games { get; set; } = [];

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrWhiteSpace(WebsiteUrl) && !WebUrl.IsValid(WebsiteUrl))
            {
                yield return new ValidationResult(
                    "Enter a full web address starting with http:// or https://.",
                    [nameof(WebsiteUrl)]);
            }

            if (!string.IsNullOrWhiteSpace(City) && IndianCity.Find(City) is null)
            {
                yield return new ValidationResult(
                    "Choose a city from the list.",
                    [nameof(City)]);
            }

            if (!string.IsNullOrWhiteSpace(GstNumber) && !StudioProfile.IsValidGstNumber(GstNumber))
            {
                yield return new ValidationResult(
                    "Enter a valid 15-character GSTIN.",
                    [nameof(GstNumber)]);
            }
        }

        public static InputModel From(StudioProfileDto profile) => new()
        {
            Name = profile.Name,
            WebsiteUrl = profile.WebsiteUrl,
            City = profile.City,
            Size = profile.Size,
            Type = profile.Type,
            About = profile.About,
            GstNumber = profile.GstNumber,
            EngineIds = profile.Engines.Select(x => x.Id).ToList(),
            Genres = profile.Genres.ToList(),
            CultureTags = profile.CultureTags.ToList(),
            Games = profile.Games
                .Select(x => new StudioGameInput { Name = x.Name, Year = x.Year, Url = x.Url })
                .ToList(),
        };

        /// <summary>Only called after validation, so required enum values are present.</summary>
        public SaveStudioProfileCommand ToCommand(Guid userId) => new(
            userId,
            Name,
            WebsiteUrl,
            City,
            Size!.Value,
            Type!.Value,
            About,
            GstNumber,
            EngineIds,
            Genres,
            CultureTags,
            Games.Select(x => x.ToDetails()).ToList());
    }
}
