namespace HireGameDev.Application;

/// <summary>
/// Outcome of a command that can fail for reasons the user must see (not exceptions).
/// Errors are plain sentences, ready for a validation summary.
/// </summary>
public sealed record OperationResult(
    bool Succeeded,
    IReadOnlyCollection<string> Errors)
{
    public static OperationResult Success() =>
        new(true, []);

    public static OperationResult Failure(params string[] errors) =>
        new(false, errors);

    public static OperationResult Failure(IReadOnlyCollection<string> errors) =>
        new(false, errors);
}
