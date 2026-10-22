using HireGameDev.Application.Persistence;

namespace HireGameDev.Application.Features.Jobs.ChangeStudioJobStatus;

public sealed class ChangeStudioJobStatusHandler(
    IStudioProfileRepository studioProfiles,
    IJobRepository jobs,
    IApplicationUnitOfWork unitOfWork)
{
    public async Task<OperationResult> HandleAsync(
        ChangeStudioJobStatusCommand command,
        CancellationToken cancellationToken = default)
    {
        var studioProfileId = await studioProfiles.GetIdByUserIdAsync(
            command.UserId,
            cancellationToken);

        if (studioProfileId is null)
        {
            return OperationResult.Failure("Create a studio profile before managing jobs.");
        }

        var job = await jobs.GetForStudioUpdateAsync(
            studioProfileId.Value,
            command.JobId,
            cancellationToken);

        if (job is null)
        {
            return OperationResult.Failure("The job could not be found.");
        }

        var nowUtc = DateTime.UtcNow;

        if (job.ExpireIfDue(nowUtc))
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return OperationResult.Failure("This job has expired and is no longer active.");
        }

        try
        {
            switch (command.Action)
            {
                case StudioJobLifecycleAction.Publish:
                    if (command.ExpiresAtUtc is null)
                    {
                        return OperationResult.Failure("Choose an expiry date before publishing.");
                    }

                    job.Publish(command.ExpiresAtUtc.Value);
                    break;

                case StudioJobLifecycleAction.Pause:
                    job.Pause();
                    break;

                case StudioJobLifecycleAction.Resume:
                    job.Resume();
                    break;

                case StudioJobLifecycleAction.Close:
                    job.Close();
                    break;

                default:
                    return OperationResult.Failure("That job action is not available.");
            }
        }
        catch (ArgumentException)
        {
            return OperationResult.Failure("Choose an expiry date in the future.");
        }
        catch (InvalidOperationException exception)
        {
            return OperationResult.Failure(exception.Message);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return OperationResult.Success();
    }
}
