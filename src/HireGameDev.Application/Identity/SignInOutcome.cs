namespace HireGameDev.Application.Identity;

public enum SignInOutcome
{
    Succeeded,

    /// <summary>Unknown user or wrong credentials. Deliberately indistinguishable.</summary>
    Failed,

    LockedOut,

    /// <summary>Credentials were valid, but the account's email has not been confirmed yet.</summary>
    EmailNotConfirmed
}
