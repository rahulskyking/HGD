using HireGameDev.Application.Features.Authentication.Commands.ResendConfirmationEmail;
using HireGameDev.Application.Tests.Features.Authentication.Fakes;

namespace HireGameDev.Application.Tests.Features.Authentication;

public class ResendConfirmationEmailHandlerTests
{
    private const string ConfirmEmailUrl = "https://hiregamedev.test/Account/ConfirmEmail";

    private readonly FakeIdentityService _identity = new();
    private readonly FakeEmailSender _email = new();
    private readonly ResendConfirmationEmailHandler _handler;

    public ResendConfirmationEmailHandlerTests()
    {
        _handler = new ResendConfirmationEmailHandler(_identity, _email);
    }

    [Fact]
    public async Task Sends_new_link_to_unconfirmed_account()
    {
        var account = _identity.AddAccount("dev@example.com", emailConfirmed: false);

        await _handler.HandleAsync(new ResendConfirmationEmailCommand("DEV@example.com", ConfirmEmailUrl));

        var message = Assert.Single(_email.Sent);
        Assert.Equal("dev@example.com", message.ToEmail);
        Assert.Contains($"userId={account.Id}&token={FakeIdentityService.Token}", message.TextBody);
    }

    [Fact]
    public async Task Does_nothing_for_unknown_email()
    {
        await _handler.HandleAsync(new ResendConfirmationEmailCommand("nobody@example.com", ConfirmEmailUrl));

        Assert.Empty(_email.Sent);
        Assert.Empty(_identity.TokenRequests);
    }

    [Fact]
    public async Task Does_nothing_for_already_confirmed_account()
    {
        _identity.AddAccount("dev@example.com", emailConfirmed: true);

        await _handler.HandleAsync(new ResendConfirmationEmailCommand("dev@example.com", ConfirmEmailUrl));

        Assert.Empty(_email.Sent);
        Assert.Empty(_identity.TokenRequests);
    }
}
