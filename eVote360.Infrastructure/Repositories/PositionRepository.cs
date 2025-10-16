using EVote360.Application.Abstractions.Repositories;
using EVote360.Domain.Entities.Position;
using EVote360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EVote360.Infrastructure.Repositories;

public sealed class PositionRepository : IPositionRepository
{
    private readonly AppDbContext _db;
    public PositionRepository(AppDbContext db) => _db = db;

    public Task<Position?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _db.Positions.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<IReadOnlyList<Position>> ListAsync(CancellationToken ct = default)
        => await _db.Positions.AsNoTracking().OrderBy(p => p.Name).ToListAsync(ct);
}
