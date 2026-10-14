using HireGameDev.Application.Features.Authentication.Commands.RegisterUser;
using HireGameDev.Application.Tests.Features.Authentication.Fakes;
using HireGameDev.Domain.Enums;

namespace HireGameDev.Application.Tests.Features.Authentication;

public class RegisterUserHandlerTests
{
    private const string ConfirmEmailUrl = "https://hiregamedev.test/Account/ConfirmEmail";

    private readonly FakeIdentityService _identity = new();
    private readonly FakeEmailSender _email = new();
    private readonly RegisterUserHandler _handler;

    public RegisterUserHandlerTests()
    {
        _handler = new RegisterUserHandler(_identity, _email);
    }

    [Fact]
    public async Task Creates_user_and_sends_confirmation_link()
    {
        var command = new RegisterUserCommand("dev@example.com", "Passw0rd!", UserRole.Candidate, ConfirmEmailUrl);

        var result = await _handler.HandleAsync(command);

        Assert.True(result.Succeeded);
        Assert.Single(_identity.CreatedUsers);
        Assert.Equal(("dev@example.com", "Passw0rd!", UserRole.Candidate), _identity.CreatedUsers[0]);

        var userId = _identity.AccountsByEmail["dev@example.com"].Id;
        Assert.Equal([userId], _identity.TokenRequests);

        var message = Assert.Single(_email.Sent);
        Assert.Equal("dev@example.com", message.ToEmail);
        var expectedLink = $"{ConfirmEmailUrl}?userId={userId}&token={FakeIdentityService.Token}";
        Assert.Contains(expectedLink, message.TextBody);
        Assert.Contains(expectedLink.Replace("&", "&amp;"), message.HtmlBody);
    }

    [Fact]
    public async Task Rejects_admin_role_without_touching_identity()
    {
        var command = new RegisterUserCommand("admin@example.com", "Passw0rd!", UserRole.Admin, ConfirmEmailUrl);

        var result = await _handler.HandleAsync(command);

        Assert.False(result.Succeeded);
        Assert.Empty(_identity.CreatedUsers);
        Assert.Empty(_email.Sent);
    }

    [Fact]
    public async Task Returns_identity_errors_and_sends_nothing_when_creation_fails()
    {
        _identity.CreateUserErrors = ["Email 'dev@example.com' is already taken."];
        var command = new RegisterUserCommand("dev@example.com", "Passw0rd!", UserRole.Studio, ConfirmEmailUrl);

        var result = await _handler.HandleAsync(command);

        Assert.False(result.Succeeded);
        Assert.Equal(["Email 'dev@example.com' is already taken."], result.Errors);
        Assert.Empty(_identity.TokenRequests);
        Assert.Empty(_email.Sent);
    }
}
