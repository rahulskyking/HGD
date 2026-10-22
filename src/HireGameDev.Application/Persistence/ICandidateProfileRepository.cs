using HireGameDev.Domain.Entities;

namespace HireGameDev.Application.Persistence;

public interface ICandidateProfileRepository
{
    Task<CandidateProfile?> GetForDisplayAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<CandidateProfile?> GetForProfileEditAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<CandidateProfile?> GetForMediaUpdateAsync(Guid userId, CancellationToken cancellationToken = default);

    void Add(CandidateProfile profile);
}
