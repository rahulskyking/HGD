using System.ComponentModel.DataAnnotations;
using HireGameDev.Application.Features.Authentication.Commands.RegisterUser;
using HireGameDev.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HireGameDev.Web.Pages.Account;

[AllowAnonymous]
public class RegisterModel : PageModel
{
    private readonly RegisterUserHandler _registerUser;

    public RegisterModel(RegisterUserHandler registerUser)
    {
        _registerUser = registerUser;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ReturnUrl { get; set; }

    public class InputModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "The {0} must be at least {2} characters long.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare(nameof(Password), ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please choose whether you are a candidate or a studio.")]
        [Display(Name = "I am a")]
        public UserRole? Role { get; set; }

        [Display(Name = "I confirm that I am 18 years of age or older")]
        public bool IsAdult { get; set; }

        [Display(Name = "I agree to the Terms of Service and Privacy Policy")]
        public bool AcceptsTerms { get; set; }
    }

    public IActionResult OnGet(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToPage("/Index");
        }

        ReturnUrl = returnUrl;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(
        string? returnUrl = null,
        CancellationToken cancellationToken = default)
    {
        ReturnUrl = returnUrl;

        if (!Input.IsAdult)
        {
            ModelState.AddModelError("Input.IsAdult", "You must be 18 or older to use HireGameDev.");
        }

        if (!Input.AcceptsTerms)
        {
            ModelState.AddModelError("Input.AcceptsTerms", "You must accept the Terms of Service and Privacy Policy.");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var confirmEmailUrl = Url.Page(
            "/Account/ConfirmEmail",
            pageHandler: null,
            values: null,
            protocol: Request.Scheme)!;

        var result = await _registerUser.HandleAsync(
            new RegisterUserCommand(Input.Email, Input.Password, Input.Role!.Value, confirmEmailUrl),
            cancellationToken);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return Page();
        }

        return RedirectToPage("./RegisterConfirmation", new { email = Input.Email });
    }
}
