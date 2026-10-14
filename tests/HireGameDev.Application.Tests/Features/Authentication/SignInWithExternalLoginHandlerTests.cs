using HireGameDev.Application.Features.Authentication.Commands.SignInWithExternalLogin;
using HireGameDev.Application.Identity;
using HireGameDev.Application.Tests.Features.Authentication.Fakes;

namespace HireGameDev.Application.Tests.Features.Authentication;

public class SignInWithExternalLoginHandlerTests
{
    private static readonly ExternalLoginDetails GoogleLogin =
        new("Google", "Google", "google-123", "dev@example.com", "Dev Example");

    private readonly FakeIdentityService _identity = new();
    private readonly SignInWithExternalLoginHandler _handler;

    public SignInWithExternalLoginHandlerTests()
    {
        _handler = new SignInWithExternalLoginHandler(_identity);
    }

    [Fact]
    public async Task Fails_when_no_external_login_is_pending()
    {
        var result = await _handler.HandleAsync();

        Assert.Equal(ExternalSignInStatus.Failed, result.Status);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public async Task Signs_in_when_login_is_already_linked()
    {
        _identity.PendingExternalLogin = GoogleLogin;
        _identity.ExternalLoginSignInOutcome = SignInOutcome.Succeeded;

        var result = await _handler.HandleAsync();

        Assert.Equal(ExternalSignInStatus.SignedIn, result.Status);
        Assert.Empty(_identity.AddedExternalLogins);
    }

    [Fact]
    public async Task Reports_locked_out_account()
    {
        _identity.PendingExternalLogin = GoogleLogin;
        _identity.ExternalLoginSignInOutcome = SignInOutcome.LockedOut;

        var result = await _handler.HandleAsync();

        Assert.Equal(ExternalSignInStatus.LockedOut, result.Status);
    }

    [Fact]
    public async Task Requires_registration_for_unknown_email()
    {
        _identity.PendingExternalLogin = GoogleLogin;

        var result = await _handler.HandleAsync();

        Assert.Equal(ExternalSignInStatus.RegistrationRequired, result.Status);
        Assert.Equal(GoogleLogin, result.ExternalLogin);
        Assert.Empty(_identity.SignIns);
    }

    [Fact]
    public async Task Links_login_and_signs_in_existing_confirmed_account()
    {
        var account = _identity.AddAccount("dev@example.com", emailConfirmed: true);
        _identity.PendingExternalLogin = GoogleLogin;

        var result = await _handler.HandleAsync();

        Assert.Equal(ExternalSignInStatus.SignedIn, result.Status);
        Assert.Equal([(account.Id, GoogleLogin)], _identity.AddedExternalLogins);
        Assert.Equal([(account.Id, false)], _identity.SignIns);
    }

    [Fact]
    public async Task Refuses_to_link_to_unconfirmed_account()
    {
        _identity.AddAccount("dev@example.com", emailConfirmed: false);
        _identity.PendingExternalLogin = GoogleLogin;

        var result = await _handler.HandleAsync();

        Assert.Equal(ExternalSignInStatus.Failed, result.Status);
        Assert.Empty(_identity.AddedExternalLogins);
        Assert.Empty(_identity.SignIns);
    }

    [Fact]
    public async Task Fails_when_provider_shares_no_email()
    {
        _identity.PendingExternalLogin = GoogleLogin with { Email = null };

        var result = await _handler.HandleAsync();

        Assert.Equal(ExternalSignInStatus.Failed, result.Status);
        Assert.Contains("email", result.Error, StringComparison.OrdinalIgnoreCase);
    }
}
