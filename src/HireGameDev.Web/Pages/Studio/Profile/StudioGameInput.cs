using System.ComponentModel.DataAnnotations;
using HireGameDev.Domain.Entities;
using HireGameDev.Domain;

namespace HireGameDev.Web.Pages.Studio.Profile;

/// <summary>One row of the studio's released-games repeater.</summary>
public class StudioGameInput : IValidatableObject
{
    [Required]
    [StringLength(StudioGame.NameMaxLength)]
    [Display(Name = "Game title")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Release year")]
    public int? Year { get; set; }

    [StringLength(StudioGame.UrlMaxLength)]
    [Display(Name = "Store page or trailer (optional)")]
    public string? Url { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Year is { } year && (year < StudioGame.EarliestYear || year > StudioGame.LatestYear()))
        {
            yield return new ValidationResult(
                $"Year must be between {StudioGame.EarliestYear} and {StudioGame.LatestYear()}.",
                [nameof(Year)]);
        }

        if (!string.IsNullOrWhiteSpace(Url) && !WebUrl.IsValid(Url))
        {
            yield return new ValidationResult(
                "Enter a full web address starting with http:// or https://.",
                [nameof(Url)]);
        }
    }

    public StudioGameDetails ToDetails() => new(Name, Year!.Value, Url);
}
