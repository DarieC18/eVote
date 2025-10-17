using EVote360.Domain.Entities;
using EVote360.Domain.Entities.Candidate;

namespace EVote360.Application.Abstractions.Repositories;

public interface ICandidateRepository
{
    Task<Candidate?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Candidate>> ListByPartyAsync(Guid partyId, CancellationToken ct = default);
    Task AddAsync(Candidate entity, CancellationToken ct = default);
    Task UpdateAsync(Candidate entity, CancellationToken ct = default);
    Task DeleteAsync(Candidate entity, CancellationToken ct = default);
}
