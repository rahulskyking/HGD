using HireGameDev.Application.Features.Authentication.Commands.CompleteExternalRegistration;
using HireGameDev.Application.Identity;
using HireGameDev.Application.Tests.Features.Authentication.Fakes;
using HireGameDev.Domain.Enums;

namespace HireGameDev.Application.Tests.Features.Authentication;

public class CompleteExternalRegistrationHandlerTests
{
    private static readonly ExternalLoginDetails GoogleLogin =
        new("Google", "Google", "google-123", "dev@example.com", "Dev Example");

    private readonly FakeIdentityService _identity = new();
    private readonly CompleteExternalRegistrationHandler _handler;

    public CompleteExternalRegistrationHandlerTests()
    {
        _handler = new CompleteExternalRegistrationHandler(_identity);
    }

    [Fact]
    public async Task Creates_confirmed_user_with_chosen_role_and_signs_in()
    {
        _identity.PendingExternalLogin = GoogleLogin;

        var result = await _handler.HandleAsync(new CompleteExternalRegistrationCommand(UserRole.Studio));

        Assert.True(result.Succeeded);
        var created = Assert.Single(_identity.CreatedExternalUsers);
        Assert.Equal(("dev@example.com", UserRole.Studio, GoogleLogin), created);
        var account = _identity.AccountsByEmail["dev@example.com"];
        Assert.Equal([(account.Id, false)], _identity.SignIns);
    }

    [Fact]
    public async Task Rejects_admin_role()
    {
        _identity.PendingExternalLogin = GoogleLogin;

        var result = await _handler.HandleAsync(new CompleteExternalRegistrationCommand(UserRole.Admin));

        Assert.False(result.Succeeded);
        Assert.Empty(_identity.CreatedExternalUsers);
    }

    [Fact]
    public async Task Fails_when_external_login_expired()
    {
        var result = await _handler.HandleAsync(new CompleteExternalRegistrationCommand(UserRole.Candidate));

        Assert.False(result.Succeeded);
        Assert.Empty(_identity.CreatedExternalUsers);
        Assert.Empty(_identity.SignIns);
    }

    [Fact]
    public async Task Fails_when_email_already_registered()
    {
        _identity.AddAccount("dev@example.com", emailConfirmed: true);
        _identity.PendingExternalLogin = GoogleLogin;

        var result = await _handler.HandleAsync(new CompleteExternalRegistrationCommand(UserRole.Candidate));

        Assert.False(result.Succeeded);
        Assert.Empty(_identity.CreatedExternalUsers);
        Assert.Empty(_identity.SignIns);
    }
}
