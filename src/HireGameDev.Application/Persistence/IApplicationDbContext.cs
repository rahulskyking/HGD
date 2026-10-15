using HireGameDev.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HireGameDev.Application.Persistence;

/// <summary>
/// The data-access port (ADR-009): handlers query and change aggregates through it and call
/// <see cref="SaveChangesAsync"/> once per use case. Only the sets handlers need are exposed;
/// child entities (skills, shipped titles, links) are reached through their aggregate.
/// Implemented by Infrastructure's ApplicationDbContext.
/// </summary>
public interface IApplicationDbContext
{
    DbSet<CandidateProfile> CandidateProfiles { get; }

    DbSet<Skill> Skills { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
