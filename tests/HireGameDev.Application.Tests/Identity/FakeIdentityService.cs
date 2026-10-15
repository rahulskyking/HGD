using HireGameDev.Application.Identity;
using HireGameDev.Domain.Enums;

namespace HireGameDev.Application.Tests.Identity;

/// <summary>
/// In-memory stand-in for the Identity adapter. Behaves like Identity where it matters:
/// an external login can only sign in once it is linked to an account.
/// </summary>
internal sealed class FakeIdentityService : IIdentityService
{
    public Dictionary<string, UserAccount> AccountsByEmail { get; } =
        new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Linked logins and what signing in with them yields. Unlinked logins fail.</summary>
    public Dictionary<(string Provider, string ProviderKey), SignInOutcome> LinkedLogins { get; } = new();

    public List<(string Email, UserRole Role, ExternalLoginDetails Login)> CreatedExternalUsers { get; } = [];

    public List<(Guid UserId, ExternalLoginDetails Login)> AddedExternalLogins { get; } = [];

    public List<(string Provider, string ProviderKey)> SignInAttempts { get; } = [];

    public int SignOutCalls { get; private set; }

    public ExternalLoginDetails? PendingExternalLogin { get; set; }

    /// <summary>When set, CreateExternalUserAsync fails with these errors.</summary>
    public string[]? CreateUserErrors { get; set; }

    public string[]? AddExternalLoginErrors { get; set; }

    public UserAccount AddAccount(string email, bool emailConfirmed)
    {
        var account = new UserAccount(Guid.NewGuid(), email, emailConfirmed);
        AccountsByEmail[email] = account;
        return account;
    }

    public void Link(ExternalLoginDetails login, SignInOutcome outcome = SignInOutcome.Succeeded) =>
        LinkedLogins[(login.Provider, login.ProviderKey)] = outcome;

    public Task<ExternalLoginDetails?> GetExternalLoginInfoAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(PendingExternalLogin);

    public Task<SignInOutcome> ExternalLoginSignInAsync(
        string provider, string providerKey, CancellationToken cancellationToken = default)
    {
        SignInAttempts.Add((provider, providerKey));

        return Task.FromResult(LinkedLogins.GetValueOrDefault((provider, providerKey), SignInOutcome.Failed));
    }

    public Task<UserAccount?> FindUserByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        Task.FromResult(AccountsByEmail.GetValueOrDefault(email));

    public Task<OperationResult> CreateExternalUserAsync(
        string email, UserRole role, ExternalLoginDetails externalLogin, CancellationToken cancellationToken = default)
    {
        if (CreateUserErrors is not null)
        {
            return Task.FromResult(OperationResult.Failure(CreateUserErrors));
        }

        CreatedExternalUsers.Add((email, role, externalLogin));
        AddAccount(email, emailConfirmed: true);
        Link(externalLogin);

        return Task.FromResult(OperationResult.Success());
    }

    public Task<OperationResult> AddExternalLoginAsync(
        Guid userId, ExternalLoginDetails externalLogin, CancellationToken cancellationToken = default)
    {
        if (AddExternalLoginErrors is not null)
        {
            return Task.FromResult(OperationResult.Failure(AddExternalLoginErrors));
        }

        AddedExternalLogins.Add((userId, externalLogin));
        Link(externalLogin);

        return Task.FromResult(OperationResult.Success());
    }

    public Task SignOutAsync(CancellationToken cancellationToken = default)
    {
        SignOutCalls++;
        return Task.CompletedTask;
    }
}
