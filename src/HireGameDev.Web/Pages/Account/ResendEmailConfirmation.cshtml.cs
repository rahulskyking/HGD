using System.ComponentModel.DataAnnotations;
using HireGameDev.Application.Features.Authentication.Commands.ResendConfirmationEmail;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HireGameDev.Web.Pages.Account;

[AllowAnonymous]
public class ResendEmailConfirmationModel : PageModel
{
    private readonly ResendConfirmationEmailHandler _resendConfirmationEmail;

    public ResendEmailConfirmationModel(ResendConfirmationEmailHandler resendConfirmationEmail)
    {
        _resendConfirmationEmail = resendConfirmationEmail;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public bool EmailSent { get; private set; }

    public class InputModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;
    }

    public void OnGet(string? email = null)
    {
        if (!string.IsNullOrWhiteSpace(email))
        {
            Input.Email = email;
        }
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var confirmEmailUrl = Url.Page(
            "/Account/ConfirmEmail",
            pageHandler: null,
            values: null,
            protocol: Request.Scheme)!;

        await _resendConfirmationEmail.HandleAsync(
            new ResendConfirmationEmailCommand(Input.Email, confirmEmailUrl),
            cancellationToken);

        // Same response whether or not the account exists.
        EmailSent = true;

        return Page();
    }
}
