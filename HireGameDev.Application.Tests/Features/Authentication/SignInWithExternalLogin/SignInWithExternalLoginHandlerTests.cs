using HireGameDev.Application.Features.Authentication.SignInWithExternalLogin;
using HireGameDev.Application.Identity;
using HireGameDev.Application.Tests.Identity;
using HireGameDev.Domain.Enums;

namespace HireGameDev.Application.Tests.Features.Authentication.SignInWithExternalLogin;

public class SignInWithExternalLoginHandlerTests
{
    private static readonly ExternalLoginDetails GoogleLogin =
        new("Google", "Google", "google-123", "dev@example.com", "Dev Example");

    private static readonly SignInWithExternalLoginCommand NoRole = new(null);
    private static readonly SignInWithExternalLoginCommand AsCandidate = new(UserRole.Candidate);

    private readonly FakeIdentityService _identity = new();
    private readonly SignInWithExternalLoginHandler _handler;

    public SignInWithExternalLoginHandlerTests()
    {
        _handler = new SignInWithExternalLoginHandler(_identity);
    }

    [Fact]
    public async Task Fails_when_no_external_login_is_pending()
    {
        var result = await _handler.HandleAsync(NoRole);

        Assert.Equal(ExternalSignInStatus.Failed, result.Status);
        Assert.NotNull(result.Error);
        Assert.Empty(_identity.SignInAttempts);
    }

    [Fact]
    public async Task Signs_in_when_login_is_already_linked()
    {
        _identity.PendingExternalLogin = GoogleLogin;
        _identity.Link(GoogleLogin);

        var result = await _handler.HandleAsync(NoRole);

        Assert.Equal(ExternalSignInStatus.SignedIn, result.Status);
        Assert.Empty(_identity.AddedExternalLogins);
        Assert.Empty(_identity.CreatedExternalUsers);
    }

    [Fact]
    public async Task Linked_account_wins_over_role_given_on_sign_up()
    {
        _identity.PendingExternalLogin = GoogleLogin;
        _identity.Link(GoogleLogin);

        var result = await _handler.HandleAsync(new SignInWithExternalLoginCommand(UserRole.Studio));

        Assert.Equal(ExternalSignInStatus.SignedIn, result.Status);
        Assert.Empty(_identity.CreatedExternalUsers);
    }

    [Fact]
    public async Task Reports_locked_out_account_as_failure()
    {
        _identity.PendingExternalLogin = GoogleLogin;
        _identity.Link(GoogleLogin, SignInOutcome.LockedOut);

        var result = await _handler.HandleAsync(NoRole);

        Assert.Equal(ExternalSignInStatus.Failed, result.Status);
        Assert.Contains("locked", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Fails_when_provider_shares_no_email()
    {
        _identity.PendingExternalLogin = GoogleLogin with { Email = null };

        var result = await _handler.HandleAsync(AsCandidate);

        Assert.Equal(ExternalSignInStatus.Failed, result.Status);
        Assert.Contains("email", result.Error, StringComparison.OrdinalIgnoreCase);
        Assert.Empty(_identity.CreatedExternalUsers);
    }

    [Fact]
    public async Task Asks_for_a_role_when_email_is_unknown_and_none_was_given()
    {
        _identity.PendingExternalLogin = GoogleLogin;

        var result = await _handler.HandleAsync(NoRole);

        Assert.Equal(ExternalSignInStatus.RegistrationRequired, result.Status);
        Assert.Equal(GoogleLogin, result.ExternalLogin);
        Assert.Empty(_identity.CreatedExternalUsers);
    }

    [Fact]
    public async Task Creates_account_with_chosen_role_and_signs_in()
    {
        _identity.PendingExternalLogin = GoogleLogin;

        var result = await _handler.HandleAsync(AsCandidate);

        Assert.Equal(ExternalSignInStatus.SignedIn, result.Status);
        Assert.Equal([("dev@example.com", UserRole.Candidate, GoogleLogin)], _identity.CreatedExternalUsers);
        Assert.Equal(("Google", "google-123"), _identity.SignInAttempts.Last());
    }

    [Fact]
    public async Task Rejects_role_that_cannot_self_register()
    {
        _identity.PendingExternalLogin = GoogleLogin;

        var result = await _handler.HandleAsync(new SignInWithExternalLoginCommand(UserRole.Admin));

        Assert.Equal(ExternalSignInStatus.Failed, result.Status);
        Assert.Empty(_identity.CreatedExternalUsers);
    }

    [Fact]
    public async Task Reports_account_creation_errors()
    {
        _identity.PendingExternalLogin = GoogleLogin;
        _identity.CreateUserErrors = ["Email 'dev@example.com' is invalid."];

        var result = await _handler.HandleAsync(AsCandidate);

        Assert.Equal(ExternalSignInStatus.Failed, result.Status);
        Assert.Equal("Email 'dev@example.com' is invalid.", result.Error);
    }

    [Fact]
    public async Task Links_login_and_signs_in_existing_confirmed_account()
    {
        var account = _identity.AddAccount("dev@example.com", emailConfirmed: true);
        _identity.PendingExternalLogin = GoogleLogin;

        var result = await _handler.HandleAsync(AsCandidate);

        Assert.Equal(ExternalSignInStatus.SignedIn, result.Status);
        Assert.Equal([(account.Id, GoogleLogin)], _identity.AddedExternalLogins);
        Assert.Empty(_identity.CreatedExternalUsers);
    }

    [Fact]
    public async Task Refuses_to_link_to_unconfirmed_account()
    {
        _identity.AddAccount("dev@example.com", emailConfirmed: false);
        _identity.PendingExternalLogin = GoogleLogin;

        var result = await _handler.HandleAsync(AsCandidate);

        Assert.Equal(ExternalSignInStatus.Failed, result.Status);
        Assert.Empty(_identity.AddedExternalLogins);
        Assert.Single(_identity.SignInAttempts);
    }

    [Fact]
    public async Task Reports_linking_errors()
    {
        _identity.AddAccount("dev@example.com", emailConfirmed: true);
        _identity.PendingExternalLogin = GoogleLogin;
        _identity.AddExternalLoginErrors = ["Login already associated with another account."];

        var result = await _handler.HandleAsync(NoRole);

        Assert.Equal(ExternalSignInStatus.Failed, result.Status);
        Assert.Equal("Login already associated with another account.", result.Error);
    }
}
