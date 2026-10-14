namespace HireGameDev.Application.Identity;

/// <summary>
/// Identity asserted by an external provider (Google, GitHub, ...).
/// </summary>
/// <param name="Provider">Authentication scheme name, e.g. "Google".</param>
/// <param name="ProviderDisplayName">Human-readable provider name for UI.</param>
/// <param name="ProviderKey">The user's stable id at the provider.</param>
/// <param name="Email">Email shared by the provider, if any.</param>
/// <param name="DisplayName">Name shared by the provider, if any.</param>
public sealed record ExternalLoginDetails(
    string Provider,
    string ProviderDisplayName,
    string ProviderKey,
    string? Email,
    string? DisplayName);
