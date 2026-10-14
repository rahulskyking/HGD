namespace HireGameDev.Infrastructure.Email;

/// <summary>
/// Bound from "Email:Resend". The API key must come from user secrets / environment, never from source.
/// </summary>
public sealed class ResendOptions
{
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>Sender address on a domain verified in Resend.</summary>
    public string FromAddress { get; set; } = string.Empty;

    public string FromName { get; set; } = "HireGameDev";
}
