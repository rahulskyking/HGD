namespace HireGameDev.Domain.Enums;

/// <summary>
/// The kind of account a user holds. Exactly one role is assigned at registration.
/// The member names are also the ASP.NET Identity role names.
/// </summary>
public enum UserRole
{
    Candidate = 1,
    Studio = 2,
    Admin = 3
}
