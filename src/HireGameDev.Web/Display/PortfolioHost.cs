namespace HireGameDev.Web.Display;

/// <summary>Text to show for a portfolio link: the candidate's label, else the site name.</summary>
public static class PortfolioHost
{
    public static string Label(string url, string? label)
    {
        if (!string.IsNullOrWhiteSpace(label))
        {
            return label;
        }

        if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            var host = uri.Host;

            return host.StartsWith("www.", StringComparison.OrdinalIgnoreCase) ? host[4..] : host;
        }

        return url;
    }
}
