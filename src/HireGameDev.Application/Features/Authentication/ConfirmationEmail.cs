using System.Net;
using HireGameDev.Application.Email;

namespace HireGameDev.Application.Features.Authentication;

/// <summary>
/// Builds the "confirm your email" message used by registration and by resend.
/// </summary>
internal static class ConfirmationEmail
{
    public const string Subject = "Confirm your HireGameDev email";

    /// <param name="confirmEmailUrl">Absolute URL of the confirm-email endpoint, without user/token.</param>
    public static string BuildLink(string confirmEmailUrl, Guid userId, string token)
    {
        var separator = confirmEmailUrl.Contains('?') ? '&' : '?';

        return $"{confirmEmailUrl}{separator}userId={userId}&token={Uri.EscapeDataString(token)}";
    }

    public static EmailMessage Create(string toEmail, string confirmationLink)
    {
        var encodedLink = WebUtility.HtmlEncode(confirmationLink);

        var html = $"""
            <p>Welcome to HireGameDev!</p>
            <p>Please confirm your email address by clicking the link below:</p>
            <p><a href="{encodedLink}">Confirm my email</a></p>
            <p>If the button doesn't work, copy and paste this address into your browser:<br>{encodedLink}</p>
            <p>If you didn't create a HireGameDev account, you can safely ignore this email.</p>
            """;

        var text = $"""
            Welcome to HireGameDev!

            Please confirm your email address by opening this link:
            {confirmationLink}

            If you didn't create a HireGameDev account, you can safely ignore this email.
            """;

        return new EmailMessage(toEmail, Subject, html, text);
    }
}
