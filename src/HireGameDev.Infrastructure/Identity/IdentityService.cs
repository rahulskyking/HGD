using System.Security.Claims;
using HireGameDev.Application.Identity;
using HireGameDev.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace HireGameDev.Infrastructure.Identity;

/// <summary>
/// Adapts ASP.NET Core Identity to <see cref="IIdentityService"/>.
/// Identity's own APIs take no cancellation tokens; the tokens are accepted for contract
/// consistency and forwarded where possible.
/// </summary>
public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<ExternalLoginDetails?> GetExternalLoginInfoAsync(
        CancellationToken cancellationToken = default)
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();

        if (info is null)
        {
            return null;
        }

        return new ExternalLoginDetails(
            info.LoginProvider,
            info.ProviderDisplayName ?? info.LoginProvider,
            info.ProviderKey,
            info.Principal.FindFirstValue(ClaimTypes.Email),
            info.Principal.FindFirstValue(ClaimTypes.Name));
    }

    public async Task<SignInOutcome> ExternalLoginSignInAsync(
        string provider,
        string providerKey,
        CancellationToken cancellationToken = default)
    {
        // Persistent: people sign in with a provider once and expect to stay signed in
        // (Identity's application cookie: 14 days, sliding). Identity also clears the
        // external cookie here, so the pending login cannot be replayed.
        var result = await _signInManager.ExternalLoginSignInAsync(
            provider,
            providerKey,
            isPersistent: true,
            bypassTwoFactor: true);

        if (result.Succeeded)
        {
            return SignInOutcome.Succeeded;
        }

        if (result.IsLockedOut)
        {
            return SignInOutcome.LockedOut;
        }

        // With RequireConfirmedEmail enabled (and no 2FA), NotAllowed means "email not confirmed".
        if (result.IsNotAllowed)
        {
            return SignInOutcome.EmailNotConfirmed;
        }

        return SignInOutcome.Failed;
    }

    public async Task<UserAccount?> FindUserByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);

        return user is null
            ? null
            : new UserAccount(user.Id, user.Email!, user.EmailConfirmed);
    }

    public async Task<CreateUserResult> CreateExternalUserAsync(
        string email,
        UserRole role,
        ExternalLoginDetails externalLogin,
        CancellationToken cancellationToken = default)
    {
        // The provider verified the email, so the account starts confirmed and has no password.
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user);

        if (!result.Succeeded)
        {
            return CreateUserResult.Failure(Describe(result));
        }

        var roleResult = await _userManager.AddToRoleAsync(user, role.ToString());

        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);

            return CreateUserResult.Failure(Describe(roleResult));
        }

        var loginResult = await _userManager.AddLoginAsync(user, ToUserLoginInfo(externalLogin));

        if (!loginResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);

            return CreateUserResult.Failure(Describe(loginResult));
        }

        return CreateUserResult.Success(user.Id);
    }

    public async Task<IdentityOperationResult> AddExternalLoginAsync(
        Guid userId,
        ExternalLoginDetails externalLogin,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new InvalidOperationException($"User '{userId}' was not found.");

        var result = await _userManager.AddLoginAsync(user, ToUserLoginInfo(externalLogin));

        return result.Succeeded
            ? IdentityOperationResult.Success()
            : IdentityOperationResult.Failure(Describe(result));
    }

    public Task SignOutAsync(CancellationToken cancellationToken = default) =>
        _signInManager.SignOutAsync();

    private static UserLoginInfo ToUserLoginInfo(ExternalLoginDetails externalLogin) =>
        new(externalLogin.Provider, externalLogin.ProviderKey, externalLogin.ProviderDisplayName);

    private static string[] Describe(IdentityResult result) =>
        result.Errors.Select(x => x.Description).ToArray();
}
