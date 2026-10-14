namespace HireGameDev.Web.Configuration;

/// <summary>
/// Bound from "Authentication:{Provider}". Secrets come from user secrets / environment.
/// </summary>
public sealed class OAuthProviderSettings
{
    public string? ClientId { get; set; }

    public string? ClientSecret { get; set; }

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(ClientId)
        && !string.IsNullOrWhiteSpace(ClientSecret);
}
