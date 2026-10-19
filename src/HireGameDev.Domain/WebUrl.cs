namespace HireGameDev.Domain;

/// <summary>
/// The one rule for every link a user stores (portfolio links, studio websites, store pages):
/// an absolute http(s) address with a host, so a stored link can never run script when rendered.
/// </summary>
public static class WebUrl
{
    public const int MaxLength = 500;

    public static bool IsValid(string? url) =>
        url is not null
        && url.Length <= MaxLength
        && Uri.TryCreate(url.Trim(), UriKind.Absolute, out var uri)
        && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
        && !string.IsNullOrEmpty(uri.Host);

    /// <summary>Returns the trimmed address, or throws when it is not a valid web address.</summary>
    public static string Require(string? url, string paramName)
    {
        if (!IsValid(url))
        {
            throw new ArgumentException("Enter a full web address starting with http:// or https://.", paramName);
        }

        return url!.Trim();
    }

    /// <summary>Like <see cref="Require"/> but blank means "no link" and comes back as <c>null</c>.</summary>
    public static string? Optional(string? url, string paramName) =>
        string.IsNullOrWhiteSpace(url) ? null : Require(url, paramName);
}
