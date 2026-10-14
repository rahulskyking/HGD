namespace HireGameDev.Application.Identity;

public sealed record CreateUserResult(
    bool Succeeded,
    Guid UserId,
    IReadOnlyCollection<string> Errors)
{
    public static CreateUserResult Success(Guid userId) =>
        new(true, userId, []);

    public static CreateUserResult Failure(IReadOnlyCollection<string> errors) =>
        new(false, Guid.Empty, errors);
}
