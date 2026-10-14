using HireGameDev.Application.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HireGameDev.Web.Pages.Account;

[AllowAnonymous]
public class ConfirmEmailModel : PageModel
{
    private readonly IIdentityService _identityService;

    public ConfirmEmailModel(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public bool Succeeded { get; private set; }

    public IReadOnlyCollection<string> Errors { get; private set; } = [];

    public async Task<IActionResult> OnGetAsync(
        Guid? userId,
        string? token,
        CancellationToken cancellationToken)
    {
        if (userId is null || userId == Guid.Empty || string.IsNullOrWhiteSpace(token))
        {
            return RedirectToPage("/Index");
        }

        var result = await _identityService.ConfirmEmailAsync(userId.Value, token, cancellationToken);

        Succeeded = result.Succeeded;
        Errors = result.Errors;

        return Page();
    }
}
