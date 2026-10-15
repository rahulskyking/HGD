using System.ComponentModel.DataAnnotations;
using HireGameDev.Application.Features.Authentication.SignInWithExternalLogin;
using HireGameDev.Domain.Enums;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HireGameDev.Web.Pages.Account;

/// <summary>
/// Google / GitHub sign-in:
///   POST  /Account/ExternalLogin                       -> start the provider challenge (with the role chosen on sign-up, if any)
///   GET   /Account/ExternalLogin?handler=Callback      -> provider redirected back: sign in, or create the account with that role
///   POST  /Account/ExternalLogin?handler=Confirmation  -> a new user who arrived without a role picked one
/// </summary>
[AllowAnonymous]
public class ExternalLoginModel : PageModel
{
    private readonly SignInWithExternalLoginHandler _signInWithExternalLogin;

    public ExternalLoginModel(SignInWithExternalLoginHandler signInWithExternalLogin)
    {
        _signInWithExternalLogin = signInWithExternalLogin;
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
        [Required(ErrorMessage = "Please choose whether you are a game developer or a studio.")]
        [Display(Name = "I am a")]
        public UserRole? Role { get; set; }
    }

    public IActionResult OnGet() => RedirectToPage("./Login");

    public IActionResult OnPost(string provider, UserRole? role = null, string? returnUrl = null)
    {
        if (string.IsNullOrWhiteSpace(provider))
        {
            return RedirectToPage("./Login", new { returnUrl });
        }

        // Only self-service roles travel through the flow; anything else is asked for after the callback.
        if (role is { } chosen && !chosen.CanSelfRegister())
        {
            role = null;
        }

        // The callback URL (role included) is carried in the protected OAuth state, so it round-trips intact.
        var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl, role });

        // Equivalent of SignInManager.ConfigureExternalAuthenticationProperties: Identity reads the
        // "LoginProvider" item back from the external cookie when the callback is processed.
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        properties.Items["LoginProvider"] = provider;

        return new ChallengeResult(provider, properties);
    }

    public async Task<IActionResult> OnGetCallbackAsync(
        UserRole? role = null,
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

        var result = await _signInWithExternalLogin.HandleAsync(
            new SignInWithExternalLoginCommand(role),
            cancellationToken);

        switch (result.Status)
        {
            case ExternalSignInStatus.SignedIn:
                return RedirectToReturnUrlOrHome(returnUrl);

            case ExternalSignInStatus.RegistrationRequired:
                ProviderDisplayName = result.ExternalLogin!.ProviderDisplayName;
                Email = result.ExternalLogin.Email;
                return Page();

            default:
                ErrorMessage = result.Error;
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

        var result = await _signInWithExternalLogin.HandleAsync(
            new SignInWithExternalLoginCommand(Input.Role),
            cancellationToken);

        if (result.Status == ExternalSignInStatus.SignedIn)
        {
            return RedirectToReturnUrlOrHome(returnUrl);
        }

        ModelState.AddModelError(string.Empty, result.Error ?? "Sign-in could not be completed. Please try again.");

        return Page();
    }

    // "/Index" sends signed-in users on to their role's home page.
    private IActionResult RedirectToReturnUrlOrHome(string? returnUrl) =>
        Url.IsLocalUrl(returnUrl)
            ? LocalRedirect(returnUrl)
            : RedirectToPage("/Index");
}
