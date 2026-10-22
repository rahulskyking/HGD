using HireGameDev.Domain.Entities;

namespace HireGameDev.Application.Persistence;

/// <summary>Studio-scoped access to job aggregates.</summary>
public interface IJobRepository
{
    Task<IReadOnlyList<Job>> GetForStudioListAsync(
        Guid studioProfileId,
        CancellationToken cancellationToken = default);

    Task<Job?> GetForStudioEditAsync(
        Guid studioProfileId,
        Guid jobId,
        CancellationToken cancellationToken = default);

    Task<Job?> GetForStudioUpdateAsync(
        Guid studioProfileId,
        Guid jobId,
        CancellationToken cancellationToken = default);

    void Add(Job job);
}
