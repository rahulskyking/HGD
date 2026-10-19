using System.ComponentModel.DataAnnotations;
using HireGameDev.Application.Features.CandidateProfiles.SaveCandidateProfile;
using HireGameDev.Domain.Entities;

namespace HireGameDev.Web.ViewModels.CandidateProfile;

/// <summary>One row of the shipped-titles repeater on the edit form.</summary>
public sealed class ShippedTitleInput : IValidatableObject
{
    [Required]
    [StringLength(ShippedTitle.GameNameMaxLength)]
    [Display(Name = "Game")]
    public string GameName { get; set; } = string.Empty;

    [Required]
    [StringLength(ShippedTitle.StudioNameMaxLength)]
    [Display(Name = "Studio")]
    public string StudioName { get; set; } = string.Empty;

    [Required]
    [StringLength(ShippedTitle.RoleMaxLength)]
    [Display(Name = "Your role")]
    public string Role { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Year")]
    public int? Year { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Year is { } year && (year < ShippedTitle.EarliestYear || year > ShippedTitle.LatestYear()))
        {
            yield return new ValidationResult(
                $"Year must be between {ShippedTitle.EarliestYear} and {ShippedTitle.LatestYear()}.",
                [nameof(Year)]);
        }
    }

    public ShippedTitleDetails ToDetails() => new(GameName, StudioName, Role, Year!.Value);
}
