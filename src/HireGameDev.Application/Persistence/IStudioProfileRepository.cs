using HireGameDev.Domain.Entities;

namespace HireGameDev.Application.Persistence;

public interface IStudioProfileRepository
{
    Task<StudioProfile?> GetForDisplayAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<StudioProfile?> GetForProfileEditAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<StudioProfile?> GetForMediaUpdateAsync(Guid userId, CancellationToken cancellationToken = default);

    void Add(StudioProfile profile);
}
