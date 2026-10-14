using HireGameDev.Application.Identity;
using HireGameDev.Domain.Enums;

namespace HireGameDev.Application.Tests.Features.Authentication.Fakes;

/// <summary>
/// In-memory stand-in for the Identity adapter. Records calls so tests can assert on
/// orchestration without any framework involvement.
/// </summary>
internal sealed class FakeIdentityService : IIdentityService
{
    public const string Token = "confirmation-token";

    public Dictionary<string, UserAccount> AccountsByEmail { get; } =
        new(StringComparer.OrdinalIgnoreCase);

    public List<(string Email, string Password, UserRole Role)> CreatedUsers { get; } = [];

    public List<(string Email, UserRole Role, ExternalLoginDetails Login)> CreatedExternalUsers { get; } = [];

    public List<(Guid UserId, ExternalLoginDetails Login)> AddedExternalLogins { get; } = [];

    public List<Guid> TokenRequests { get; } = [];

    public List<(Guid UserId, bool RememberMe)> SignIns { get; } = [];

    public int SignOutCalls { get; private set; }

    /// <summary>When set, CreateUserAsync/CreateExternalUserAsync fail with these errors.</summary>
    public string[]? CreateUserErrors { get; set; }

    public string[]? AddExternalLoginErrors { get; set; }

    public ExternalLoginDetails? PendingExternalLogin { get; set; }

    public SignInOutcome ExternalLoginSignInOutcome { get; set; } = SignInOutcome.Failed;

    public SignInOutcome PasswordSignInOutcome { get; set; } = SignInOutcome.Failed;

    public UserAccount AddAccount(string email, bool emailConfirmed)
    {
        var account = new UserAccount(Guid.NewGuid(), email, emailConfirmed);
        AccountsByEmail[email] = account;
        return account;
    }

    public Task<CreateUserResult> CreateUserAsync(
        string email, string password, UserRole role, CancellationToken cancellationToken = default)
    {
        if (CreateUserErrors is not null)
        {
            return Task.FromResult(CreateUserResult.Failure(CreateUserErrors));
        }

        CreatedUsers.Add((email, password, role));
        var account = AddAccount(email, emailConfirmed: false);
        return Task.FromResult(CreateUserResult.Success(account.Id));
    }

    public Task<CreateUserResult> CreateExternalUserAsync(
        string email, UserRole role, ExternalLoginDetails externalLogin, CancellationToken cancellationToken = default)
    {
        if (CreateUserErrors is not null)
        {
            return Task.FromResult(CreateUserResult.Failure(CreateUserErrors));
        }

        CreatedExternalUsers.Add((email, role, externalLogin));
        var account = AddAccount(email, emailConfirmed: true);
        return Task.FromResult(CreateUserResult.Success(account.Id));
    }

    public Task<UserAccount?> FindUserByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        Task.FromResult(AccountsByEmail.GetValueOrDefault(email));

    public Task<string> GenerateEmailConfirmationTokenAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        TokenRequests.Add(userId);
        return Task.FromResult(Token);
    }

    public Task<IdentityOperationResult> ConfirmEmailAsync(Guid userId, string token, CancellationToken cancellationToken = default) =>
        Task.FromResult(IdentityOperationResult.Success());

    public Task<SignInOutcome> PasswordSignInAsync(
        string email, string password, bool rememberMe, CancellationToken cancellationToken = default) =>
        Task.FromResult(PasswordSignInOutcome);

    public Task<SignInOutcome> ExternalLoginSignInAsync(
        string provider, string providerKey, CancellationToken cancellationToken = default) =>
        Task.FromResult(ExternalLoginSignInOutcome);

    public Task<ExternalLoginDetails?> GetExternalLoginInfoAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(PendingExternalLogin);

    public Task<IdentityOperationResult> AddExternalLoginAsync(
        Guid userId, ExternalLoginDetails externalLogin, CancellationToken cancellationToken = default)
    {
        if (AddExternalLoginErrors is not null)
        {
            return Task.FromResult(IdentityOperationResult.Failure(AddExternalLoginErrors));
        }

        AddedExternalLogins.Add((userId, externalLogin));
        return Task.FromResult(IdentityOperationResult.Success());
    }

    public Task SignInAsync(Guid userId, bool rememberMe, CancellationToken cancellationToken = default)
    {
        SignIns.Add((userId, rememberMe));
        return Task.CompletedTask;
    }

    public Task SignOutAsync(CancellationToken cancellationToken = default)
    {
        SignOutCalls++;
        return Task.CompletedTask;
    }
}
