namespace HireGameDev.Infrastructure.Email;

/// <summary>
/// Bound from the "Email" configuration section.
/// </summary>
public sealed class EmailOptions
{
    public const string SectionName = "Email";

    public const string ResendProvider = "Resend";

    public const string LogProvider = "Log";

    /// <summary>"Resend" (default) sends real email; "Log" only writes it to the log (local development).</summary>
    public string Provider { get; set; } = ResendProvider;

    public ResendOptions Resend { get; set; } = new();
}
