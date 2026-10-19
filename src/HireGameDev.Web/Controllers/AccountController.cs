using HireGameDev.Application.Features.Authentication.SignInWithExternalLogin;
using HireGameDev.Application.Identity;
using HireGameDev.Domain.Enums;
using HireGameDev.Web.ViewModels.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HireGameDev.Web.Controllers;

[AllowAnonymous]
public sealed class AccountController(
    SignInWithExternalLoginHandler signInWithExternalLogin,
    IIdentityService identityService) : Controller
{
    [HttpGet]
    public IActionResult AccessDenied() => View();

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return Url.IsLocalUrl(returnUrl)
                ? LocalRedirect(returnUrl)
                : RedirectToHome();
        }

        return View(new LoginViewModel
        {
            ReturnUrl = returnUrl,
            ErrorMessage = TempData["ErrorMessage"] as string,
        });
    }

    [HttpGet]
    public IActionResult SignUp(UserRole? role = null, string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToHome();
        }

        return View(new SignUpViewModel
        {
            Role = role is { } chosen && chosen.CanSelfRegister() ? chosen : null,
            ReturnUrl = returnUrl,
        });
    }

    [HttpGet]
    public IActionResult ExternalLogin(string? returnUrl = null) =>
        RedirectToAction(nameof(Login), new { returnUrl });

    [HttpPost]
    public IActionResult ExternalLogin(string provider, UserRole? role = null, string? returnUrl = null)
    {
        if (string.IsNullOrWhiteSpace(provider))
        {
            return RedirectToAction(nameof(Login), new { returnUrl });
        }

        // Only self-service roles travel through the flow; anything else is chosen after callback.
        if (role is { } chosen && !chosen.CanSelfRegister())
        {
            role = null;
        }

        var redirectUrl = Url.Action(
            nameof(ExternalLoginCallback),
            "Account",
            new { returnUrl, role });

        // Identity reads "LoginProvider" back from the external cookie during callback processing.
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        properties.Items["LoginProvider"] = provider;

        return new ChallengeResult(provider, properties);
    }

    [HttpGet]
    public async Task<IActionResult> ExternalLoginCallback(
        UserRole? role = null,
        string? returnUrl = null,
        string? remoteError = null,
        CancellationToken cancellationToken = default)
    {
        if (remoteError is not null)
        {
            TempData["ErrorMessage"] = $"The sign-in provider reported an error: {remoteError}";
            return RedirectToAction(nameof(Login), new { returnUrl });
        }

        var result = await signInWithExternalLogin.HandleAsync(
            new SignInWithExternalLoginCommand(role),
            cancellationToken);

        switch (result.Status)
        {
            case ExternalSignInStatus.SignedIn:
                return RedirectToReturnUrlOrHome(returnUrl);

            case ExternalSignInStatus.RegistrationRequired:
                return View("ExternalLogin", new ExternalLoginConfirmationViewModel
                {
                    ProviderDisplayName = result.ExternalLogin!.ProviderDisplayName,
                    Email = result.ExternalLogin.Email,
                    ReturnUrl = returnUrl,
                });

            default:
                TempData["ErrorMessage"] = result.Error;
                return RedirectToAction(nameof(Login), new { returnUrl });
        }
    }

    [HttpPost]
    public async Task<IActionResult> ExternalLoginConfirmation(
        [Bind(Prefix = "")] ExternalLoginConfirmationViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View("ExternalLogin", model);
        }

        var result = await signInWithExternalLogin.HandleAsync(
            new SignInWithExternalLoginCommand(model.Input.Role),
            cancellationToken);

        if (result.Status == ExternalSignInStatus.SignedIn)
        {
            return RedirectToReturnUrlOrHome(model.ReturnUrl);
        }

        ModelState.AddModelError(
            string.Empty,
            result.Error ?? "Sign-in could not be completed. Please try again.");

        return View("ExternalLogin", model);
    }

    [HttpGet]
    public IActionResult Logout() => RedirectToHome();

    [HttpPost]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        await identityService.SignOutAsync(cancellationToken);
        return RedirectToHome();
    }

    private IActionResult RedirectToReturnUrlOrHome(string? returnUrl) =>
        Url.IsLocalUrl(returnUrl)
            ? LocalRedirect(returnUrl!)
            : RedirectToHome();

    private IActionResult RedirectToHome() =>
        User.HomePath() is { } home ? Redirect(home) : Redirect("/");
}
