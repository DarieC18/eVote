using EVote360.Domain.Entities.Citizen;
using EVote360.Domain.ValueObjects;

namespace EVote360.Application.Abstractions.Repositories;

public interface ICitizenRepository
{
    Task<Citizen?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Citizen?> GetByNationalIdAsync(NationalId id, CancellationToken ct = default);

    Task<IReadOnlyList<Citizen>> SearchAsync(string? query = null, int skip = 0, int take = 50, CancellationToken ct = default);

    Task AddAsync(Citizen entity, CancellationToken ct = default);
    Task UpdateAsync(Citizen entity, CancellationToken ct = default);
}
