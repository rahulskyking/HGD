namespace HireGameDev.Application.Identity;

public sealed record IdentityOperationResult(
    bool Succeeded,
    IReadOnlyCollection<string> Errors)
{
    public static IdentityOperationResult Success() =>
        new(true, []);

    public static IdentityOperationResult Failure(params string[] errors) =>
        new(false, errors);

    public static IdentityOperationResult Failure(IReadOnlyCollection<string> errors) =>
        new(false, errors);
}
