namespace HireGameDev.Web.ViewModels.Shared;

/// <summary>Model for <c>_StudioLogoPartial</c>: uploaded logo or branded initials fallback.</summary>
public sealed record StudioLogo(
    string Name,
    string? LogoUrl,
    StudioLogo.Size LogoSize = StudioLogo.Size.Medium)
{
    public enum Size
    {
        Small,
        Medium,
        Large,
    }

    public string Initials
    {
        get
        {
            var parts = Name.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            return parts.Length switch
            {
                0 => "?",
                1 => parts[0][..1].ToUpperInvariant(),
                _ => string.Concat(parts[0][..1], parts[^1][..1]).ToUpperInvariant(),
            };
        }
    }

    public string CssClass => LogoSize switch
    {
        Size.Small => "hgd-studio-logo hgd-studio-logo-sm",
        Size.Large => "hgd-studio-logo hgd-studio-logo-lg",
        _ => "hgd-studio-logo",
    };
}
