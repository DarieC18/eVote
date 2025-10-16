using EVote360.Domain.Entities.Position;

namespace EVote360.Application.Abstractions.Repositories;

public interface IPositionRepository
{
    Task<Position?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Position>> ListAsync(CancellationToken ct = default);
}
