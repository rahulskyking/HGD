namespace HireGameDev.Application.Identity;

public enum SignInOutcome
{
    Succeeded,

    /// <summary>No account is linked to the given login.</summary>
    Failed,

    LockedOut,

    /// <summary>The account exists but its email has not been confirmed.</summary>
    EmailNotConfirmed
}
