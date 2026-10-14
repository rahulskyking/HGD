using System.Net.Http.Json;
using System.Text.Json;
using HireGameDev.Application.Email;
using Microsoft.Extensions.Options;

namespace HireGameDev.Infrastructure.Email;

/// <summary>
/// Sends email through the Resend HTTPS API (POST /emails). Deliberately uses a plain
/// HttpClient rather than the Resend SDK; the API surface we need is a single call.
/// Never sends via SMTP from the host (see spec §20).
/// </summary>
public sealed class ResendEmailSender : IEmailSender
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _httpClient;
    private readonly ResendOptions _options;

    public ResendEmailSender(
        HttpClient httpClient,
        IOptions<ResendOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task SendAsync(
        EmailMessage message,
        CancellationToken cancellationToken = default)
    {
        var request = new SendEmailRequest(
            From: $"{_options.FromName} <{_options.FromAddress}>",
            To: [message.ToEmail],
            Subject: message.Subject,
            Html: message.HtmlBody,
            Text: message.TextBody);

        using var response = await _httpClient.PostAsJsonAsync(
            "emails",
            request,
            JsonOptions,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);

            throw new HttpRequestException(
                $"Resend rejected the email with status {(int)response.StatusCode}: {body}",
                inner: null,
                statusCode: response.StatusCode);
        }
    }

    // Serialized with camelCase property names, which is what the Resend API expects.
    private sealed record SendEmailRequest(
        string From,
        string[] To,
        string Subject,
        string Html,
        string Text);
}
