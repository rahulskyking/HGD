namespace HireGameDev.Web.ViewModels.Account;

public sealed class ExternalLoginConfirmationViewModel
{
    public ExternalLoginRoleInputModel Input { get; set; } = new();

    // Display-only values. Account identity is read from the pending external login server-side.
    public string? ProviderDisplayName { get; set; }

    public string? Email { get; set; }

    public string? ReturnUrl { get; set; }
}
