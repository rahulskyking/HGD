namespace HireGameDev.Application.Common;

public sealed record IdentityResult(
    bool Succeeded,
    IReadOnlyCollection<string> Errors)
{
    public static IdentityResult Success() =>
        new(true, []);

    public static IdentityResult Failure(
        params string[] errors) =>
        new(false, errors);
}