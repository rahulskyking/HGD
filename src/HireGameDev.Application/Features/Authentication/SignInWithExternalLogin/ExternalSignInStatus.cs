namespace HireGameDev.Application.Features.Authentication.SignInWithExternalLogin;

public enum ExternalSignInStatus
{
    SignedIn,

    /// <summary>No account exists for this external identity and no role was given; ask the user which role to create.</summary>
    RegistrationRequired,

    /// <summary>Could not sign in; <see cref="ExternalSignInResult.Error"/> explains why in user-facing terms.</summary>
    Failed
}
