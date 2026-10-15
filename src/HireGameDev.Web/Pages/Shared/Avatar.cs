namespace HireGameDev.Web.Pages.Shared;

/// <summary>Model for <c>_AvatarPartial</c>: the photo when there is one, otherwise initials.</summary>
public sealed record Avatar(string Name, string? PhotoUrl, Avatar.Size AvatarSize = Avatar.Size.Medium)
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

    public string CssClass => AvatarSize switch
    {
        Size.Small => "hgd-avatar hgd-avatar-sm",
        Size.Large => "hgd-avatar hgd-avatar-lg",
        _ => "hgd-avatar",
    };
}
