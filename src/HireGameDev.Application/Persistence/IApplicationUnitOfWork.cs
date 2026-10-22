namespace HireGameDev.Application.Persistence;

/// <summary>Commits one application use case without exposing the persistence framework.</summary>
public interface IApplicationUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
