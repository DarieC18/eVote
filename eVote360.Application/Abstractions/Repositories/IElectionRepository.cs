using EVote360.Domain.Entities.Election;

namespace EVote360.Application.Abstractions.Repositories;

public interface IElectionRepository
{
    Task<Election?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Election?> GetByYearAsync(int year, CancellationToken ct = default);
    Task<Election?> GetActiveAsync(CancellationToken ct = default);

    Task<IReadOnlyList<Election>> ListAsync(int? year = null, CancellationToken ct = default);

    Task AddAsync(Election entity, CancellationToken ct = default);
    Task UpdateAsync(Election entity, CancellationToken ct = default);
    Task DeleteAsync(Election entity, CancellationToken ct = default);
}
