using System.ComponentModel.DataAnnotations;
using HireGameDev.Application.Features.Authentication.Commands.CompleteExternalRegistration;
using HireGameDev.Application.Features.Authentication.Commands.SignInWithExternalLogin;
using HireGameDev.Domain.Enums;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HireGameDev.Web.Pages.Account;

/// <summary>
/// External (OAuth) sign-in:
///   POST  /Account/ExternalLogin                       -> start the provider challenge
///   GET   /Account/ExternalLogin?handler=Callback      -> provider redirected back
///   POST  /Account/ExternalLogin?handler=Confirmation  -> new user picked a role
/// </summary>
[AllowAnonymous]
public class ExternalLoginModel : PageModel
{
    private readonly SignInWithExternalLoginHandler _signInWithExternalLogin;
    private readonly CompleteExternalRegistrationHandler _completeExternalRegistration;

    public ExternalLoginModel(
        SignInWithExternalLoginHandler signInWithExternalLogin,
        CompleteExternalRegistrationHandler completeExternalRegistration)
    {
        _signInWithExternalLogin = signInWithExternalLogin;
        _completeExternalRegistration = completeExternalRegistration;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    // Display-only; the real identity is read from the pending external login server-side.
    [BindProperty]
    public string? ProviderDisplayName { get; set; }

    [BindProperty]
    public string? Email { get; set; }

    public string? ReturnUrl { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "Please choose whether you are a candidate or a studio.")]
        [Display(Name = "I am a")]
        public UserRole? Role { get; set; }
    }

    public IActionResult OnGet() => RedirectToPage("./Login");

    public IActionResult OnPost(string provider, string? returnUrl = null)
    {
        if (string.IsNullOrWhiteSpace(provider))
        {
            return RedirectToPage("./Login", new { returnUrl });
        }

        var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });

        // Equivalent of SignInManager.ConfigureExternalAuthenticationProperties: Identity reads the
        // "LoginProvider" item back from the external cookie when the callback is processed.
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        properties.Items["LoginProvider"] = provider;

        return new ChallengeResult(provider, properties);
    }

    public async Task<IActionResult> OnGetCallbackAsync(
        string? returnUrl = null,
        string? remoteError = null,
        CancellationToken cancellationToken = default)
    {
        ReturnUrl = returnUrl;

        if (remoteError is not null)
        {
            ErrorMessage = $"The sign-in provider reported an error: {remoteError}";

            return RedirectToPage("./Login", new { returnUrl });
        }

        var result = await _signInWithExternalLogin.HandleAsync(cancellationToken);

        switch (result.Status)
        {
            case ExternalSignInStatus.SignedIn:
                return RedirectToReturnUrlOrHome(returnUrl);

            case ExternalSignInStatus.LockedOut:
                ErrorMessage = "This account is temporarily locked after too many failed attempts. Please try again later.";
                return RedirectToPage("./Login", new { returnUrl });

            case ExternalSignInStatus.RegistrationRequired:
                ProviderDisplayName = result.ExternalLogin!.ProviderDisplayName;
                Email = result.ExternalLogin.Email;
                return Page();

            default:
                ErrorMessage = result.Error ?? "Sign-in with the external provider could not be completed.";
                return RedirectToPage("./Login", new { returnUrl });
        }
    }

    public async Task<IActionResult> OnPostConfirmationAsync(
        string? returnUrl = null,
        CancellationToken cancellationToken = default)
    {
        ReturnUrl = returnUrl;

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result = await _completeExternalRegistration.HandleAsync(
            new CompleteExternalRegistrationCommand(Input.Role!.Value),
            cancellationToken);

        if (result.Succeeded)
        {
            return RedirectToReturnUrlOrHome(returnUrl);
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error);
        }

        return Page();
    }

    private IActionResult RedirectToReturnUrlOrHome(string? returnUrl) =>
        Url.IsLocalUrl(returnUrl)
            ? LocalRedirect(returnUrl)
            : RedirectToPage("/Index");
}
