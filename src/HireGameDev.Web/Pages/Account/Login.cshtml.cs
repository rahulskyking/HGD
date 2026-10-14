using System.ComponentModel.DataAnnotations;
using HireGameDev.Application.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HireGameDev.Web.Pages.Account;

[AllowAnonymous]
public class LoginModel : PageModel
{
    private readonly IIdentityService _identityService;

    public LoginModel(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ReturnUrl { get; set; }

    /// <summary>Set by other pages (e.g. a failed external login) before redirecting here.</summary>
    [TempData]
    public string? ErrorMessage { get; set; }

    public bool EmailNotConfirmed { get; private set; }

    public class InputModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }

    public IActionResult OnGet(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToReturnUrlOrHome(returnUrl);
        }

        if (!string.IsNullOrEmpty(ErrorMessage))
        {
            ModelState.AddModelError(string.Empty, ErrorMessage);
        }

        ReturnUrl = returnUrl;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(
        string? returnUrl = null,
        CancellationToken cancellationToken = default)
    {
        ReturnUrl = returnUrl;

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var outcome = await _identityService.PasswordSignInAsync(
            Input.Email,
            Input.Password,
            Input.RememberMe,
            cancellationToken);

        switch (outcome)
        {
            case SignInOutcome.Succeeded:
                return RedirectToReturnUrlOrHome(returnUrl);

            case SignInOutcome.LockedOut:
                ModelState.AddModelError(
                    string.Empty,
                    "This account is temporarily locked after too many failed attempts. Please try again later.");
                return Page();

            case SignInOutcome.EmailNotConfirmed:
                EmailNotConfirmed = true;
                ModelState.AddModelError(
                    string.Empty,
                    "Please confirm your email address before signing in.");
                return Page();

            default:
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return Page();
        }
    }

    private IActionResult RedirectToReturnUrlOrHome(string? returnUrl) =>
        Url.IsLocalUrl(returnUrl)
            ? LocalRedirect(returnUrl)
            : RedirectToPage("/Index");
}
