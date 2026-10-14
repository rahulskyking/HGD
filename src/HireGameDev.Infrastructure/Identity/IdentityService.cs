using System.Security.Claims;
using System.Text;
using HireGameDev.Application.Identity;
using HireGameDev.Domain.Enums;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

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

    public async Task<CreateUserResult> CreateUserAsync(
        string email,
        string password,
        UserRole role,
        CancellationToken cancellationToken = default)
    {
        var user = NewUser(email, emailConfirmed: false);

        var result = await _userManager.CreateAsync(user, password);

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

        return CreateUserResult.Success(user.Id);
    }

    public async Task<CreateUserResult> CreateExternalUserAsync(
        string email,
        UserRole role,
        ExternalLoginDetails externalLogin,
        CancellationToken cancellationToken = default)
    {
        // The provider verified the email, so the account starts confirmed and has no password.
        var user = NewUser(email, emailConfirmed: true);

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

    public async Task<UserAccount?> FindUserByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);

        return user is null
            ? null
            : new UserAccount(user.Id, user.Email!, user.EmailConfirmed);
    }

    public async Task<string> GenerateEmailConfirmationTokenAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await GetRequiredUserAsync(userId);

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        // Identity tokens are not URL-safe; encode so callers can treat them as opaque strings.
        return WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
    }

    public async Task<IdentityOperationResult> ConfirmEmailAsync(
        Guid userId,
        string token,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
        {
            return IdentityOperationResult.Failure("This confirmation link is not valid.");
        }

        string decodedToken;

        try
        {
            decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
        }
        catch (FormatException)
        {
            return IdentityOperationResult.Failure("This confirmation link is not valid.");
        }

        var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

        return result.Succeeded
            ? IdentityOperationResult.Success()
            : IdentityOperationResult.Failure(Describe(result));
    }

    public async Task<SignInOutcome> PasswordSignInAsync(
        string email,
        string password,
        bool rememberMe,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            return SignInOutcome.Failed;
        }

        var result = await _signInManager.PasswordSignInAsync(
            user,
            password,
            isPersistent: rememberMe,
            lockoutOnFailure: true);

        return Map(result);
    }

    public async Task<SignInOutcome> ExternalLoginSignInAsync(
        string provider,
        string providerKey,
        CancellationToken cancellationToken = default)
    {
        var result = await _signInManager.ExternalLoginSignInAsync(
            provider,
            providerKey,
            isPersistent: false,
            bypassTwoFactor: true);

        return Map(result);
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

    public async Task<IdentityOperationResult> AddExternalLoginAsync(
        Guid userId,
        ExternalLoginDetails externalLogin,
        CancellationToken cancellationToken = default)
    {
        var user = await GetRequiredUserAsync(userId);

        var result = await _userManager.AddLoginAsync(user, ToUserLoginInfo(externalLogin));

        return result.Succeeded
            ? IdentityOperationResult.Success()
            : IdentityOperationResult.Failure(Describe(result));
    }

    public async Task SignInAsync(
        Guid userId,
        bool rememberMe,
        CancellationToken cancellationToken = default)
    {
        var user = await GetRequiredUserAsync(userId);

        await _signInManager.SignInAsync(user, isPersistent: rememberMe);

        // Unlike ExternalLoginSignInAsync, SignInAsync leaves the external cookie in place.
        // Clear it so a pending external login cannot be replayed.
        await _signInManager.Context.SignOutAsync(IdentityConstants.ExternalScheme);
    }

    public Task SignOutAsync(CancellationToken cancellationToken = default) =>
        _signInManager.SignOutAsync();

    private static ApplicationUser NewUser(string email, bool emailConfirmed) =>
        new()
        {
            Id = Guid.NewGuid(),
            UserName = email,
            Email = email,
            EmailConfirmed = emailConfirmed
        };

    private async Task<ApplicationUser> GetRequiredUserAsync(Guid userId) =>
        await _userManager.FindByIdAsync(userId.ToString())
        ?? throw new InvalidOperationException($"User '{userId}' was not found.");

    private static UserLoginInfo ToUserLoginInfo(ExternalLoginDetails externalLogin) =>
        new(externalLogin.Provider, externalLogin.ProviderKey, externalLogin.ProviderDisplayName);

    private static string[] Describe(IdentityResult result) =>
        result.Errors.Select(x => x.Description).ToArray();

    private static SignInOutcome Map(SignInResult result)
    {
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
}
