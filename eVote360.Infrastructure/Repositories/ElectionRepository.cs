using EVote360.Application.Abstractions.Repositories;
using EVote360.Domain.Entities.Election;
using EVote360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EVote360.Infrastructure.Repositories;

public sealed class ElectionRepository : IElectionRepository
{
    private readonly AppDbContext _db;
    public ElectionRepository(AppDbContext db) => _db = db;

    public Task<Election?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _db.Elections.FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<Election?> GetByYearAsync(int year, CancellationToken ct = default)
        => _db.Elections.FirstOrDefaultAsync(x => x.Year == year, ct);

    public Task<Election?> GetActiveAsync(CancellationToken ct = default)
        => _db.Elections.FirstOrDefaultAsync(x => x.Status == Domain.Enums.ElectionStatus.Active, ct);

    public async Task<IReadOnlyList<Election>> ListAsync(int? year = null, CancellationToken ct = default)
        => await _db.Elections.AsNoTracking()
            .Where(e => !year.HasValue || e.Year == year.Value)
            .OrderByDescending(e => e.Year).ToListAsync(ct);

    public async Task AddAsync(Election entity, CancellationToken ct = default)
    { _db.Elections.Add(entity); await _db.SaveChangesAsync(ct); }

    public async Task UpdateAsync(Election entity, CancellationToken ct = default)
    { _db.Elections.Update(entity); await _db.SaveChangesAsync(ct); }

    public async Task DeleteAsync(Election entity, CancellationToken ct = default)
    { _db.Elections.Remove(entity); await _db.SaveChangesAsync(ct); }
}
