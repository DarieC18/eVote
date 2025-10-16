using EVote360.Domain.Entities.Party;

namespace EVote360.Application.Abstractions.Repositories;

public interface IPartyRepository
{
    Task<Party?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Party?> GetBySiglasAsync(string siglas, CancellationToken ct = default);
    Task<IReadOnlyList<Party>> ListAsync(CancellationToken ct = default);

    Task AddAsync(Party entity, CancellationToken ct = default);
    Task UpdateAsync(Party entity, CancellationToken ct = default);
}
