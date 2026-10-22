namespace HireGameDev.Application.Features.Jobs.SaveStudioJob;

public sealed record SaveStudioJobOutcome(
    bool Succeeded,
    Guid? JobId,
    IReadOnlyCollection<string> Errors)
{
    public static SaveStudioJobOutcome Success(Guid jobId) => new(true, jobId, []);

    public static SaveStudioJobOutcome Failure(params string[] errors) => new(false, null, errors);
}
