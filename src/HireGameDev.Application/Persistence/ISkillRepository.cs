using HireGameDev.Domain.Entities;

namespace HireGameDev.Application.Persistence;

public interface ISkillRepository
{
    Task<IReadOnlyList<Skill>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Skill>> GetByIdsAsync(
        IReadOnlyCollection<Guid> skillIds,
        CancellationToken cancellationToken = default);
}
