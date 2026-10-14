namespace HireGameDev.Application.Email;

/// <summary>
/// Outbound email. Implemented in Infrastructure (Resend over HTTPS; a log-only sender for development).
/// </summary>
public interface IEmailSender
{
    Task SendAsync(
        EmailMessage message,
        CancellationToken cancellationToken = default);
}
