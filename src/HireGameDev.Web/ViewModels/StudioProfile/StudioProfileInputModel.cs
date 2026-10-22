using System.ComponentModel.DataAnnotations;
using HireGameDev.Application.Features.StudioProfiles.GetMyStudioProfile;
using HireGameDev.Application.Features.StudioProfiles.SaveStudioProfile;
using HireGameDev.Domain;
using StudioProfileEntity = global::HireGameDev.Domain.Entities.StudioProfile;
using HireGameDev.Domain.Enums;

namespace HireGameDev.Web.ViewModels.StudioProfile;

public sealed class StudioProfileInputModel : IValidatableObject
{
    [Required]
    [StringLength(StudioProfileEntity.NameMaxLength)]
    [Display(Name = "Studio name")]
    public string Name { get; set; } = string.Empty;

    [StringLength(StudioProfileEntity.WebsiteUrlMaxLength)]
    [Display(Name = "Website (optional)")]
    public string? WebsiteUrl { get; set; }

    [Required]
    [StringLength(StudioProfileEntity.CityMaxLength)]
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

    [StringLength(StudioProfileEntity.AboutMaxLength)]
    [Display(Name = "About your studio (optional)")]
    public string? About { get; set; }

    [StringLength(StudioProfileEntity.GstNumberLength)]
    [Display(Name = "GSTIN (optional)")]
    public string? GstNumber { get; set; }

    [MaxLength(StudioProfileEntity.MaximumEngines, ErrorMessage = "Pick at most {1} engines.")]
    [Display(Name = "Engines")]
    public List<Guid> EngineIds { get; set; } = [];

    [MaxLength(StudioProfileEntity.MaximumGenres, ErrorMessage = "Pick at most {1} genres.")]
    [Display(Name = "Genres")]
    public List<GameGenre> Genres { get; set; } = [];

    [MaxLength(StudioProfileEntity.MaximumCultureTags, ErrorMessage = "Pick at most {1} culture tags.")]
    [Display(Name = "Culture and benefits")]
    public List<CultureTag> CultureTags { get; set; } = [];

    [MaxLength(StudioProfileEntity.MaximumGames, ErrorMessage = "Add at most {1} released games.")]
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

        if (!string.IsNullOrWhiteSpace(GstNumber) && !StudioProfileEntity.IsValidGstNumber(GstNumber))
        {
            yield return new ValidationResult(
                "Enter a valid 15-character GSTIN.",
                [nameof(GstNumber)]);
        }
    }

    public static StudioProfileInputModel From(StudioProfileDto profile) => new()
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
