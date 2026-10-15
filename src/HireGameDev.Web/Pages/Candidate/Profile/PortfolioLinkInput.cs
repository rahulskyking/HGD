using System.ComponentModel.DataAnnotations;
using HireGameDev.Domain.Entities;

namespace HireGameDev.Web.Pages.Candidate.Profile;

/// <summary>One row of the portfolio-links repeater on the edit form.</summary>
public class PortfolioLinkInput : IValidatableObject
{
    [Required]
    [StringLength(PortfolioLink.UrlMaxLength)]
    [Display(Name = "Link")]
    public string Url { get; set; } = string.Empty;

    [StringLength(PortfolioLink.LabelMaxLength)]
    [Display(Name = "Label (optional)")]
    public string? Label { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!string.IsNullOrWhiteSpace(Url) && !PortfolioLink.IsValidUrl(Url))
        {
            yield return new ValidationResult(
                "Enter a full web address starting with http:// or https://.",
                [nameof(Url)]);
        }
    }

    public PortfolioLinkDetails ToDetails() => new(Url, Label);
}
