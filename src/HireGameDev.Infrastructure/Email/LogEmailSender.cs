using HireGameDev.Application.Email;
using Microsoft.Extensions.Logging;

namespace HireGameDev.Infrastructure.Email;

/// <summary>
/// Development-only sender: writes the message to the log instead of delivering it,
/// so confirmation links can be copied from the console.
/// </summary>
public sealed class LogEmailSender : IEmailSender
{
    private readonly ILogger<LogEmailSender> _logger;

    public LogEmailSender(ILogger<LogEmailSender> logger)
    {
        _logger = logger;
    }

    public Task SendAsync(
        EmailMessage message,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Email not sent (Email:Provider is 'Log').\nTo: {To}\nSubject: {Subject}\n\n{Body}",
            message.ToEmail,
            message.Subject,
            message.TextBody);

        return Task.CompletedTask;
    }
}
