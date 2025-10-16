using EVote360.Application.Abstractions.Repositories;
using EVote360.Domain.Entities.Citizen;
using EVote360.Domain.ValueObjects;
using EVote360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EVote360.Infrastructure.Repositories;

public sealed class CitizenRepository : ICitizenRepository
{
    private readonly AppDbContext _db;
    public CitizenRepository(AppDbContext db) => _db = db;

    public Task<Citizen?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _db.Citizens.FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<Citizen?> GetByNationalIdAsync(NationalId id, CancellationToken ct = default)
        => _db.Citizens.FirstOrDefaultAsync(x => x.NationalId == id, ct);

    public async Task<IReadOnlyList<Citizen>> SearchAsync(string? query = null, int skip = 0, int take = 50, CancellationToken ct = default)
    {
        var q = _db.Citizens.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var term = query.Trim().ToLower();
            q = q.Where(c =>
                c.FirstName.ToLower().Contains(term) ||
                c.LastName.ToLower().Contains(term) ||
                c.NationalId.Value.ToLower().Contains(term));
        }

        return await q.OrderBy(c => c.LastName).ThenBy(c => c.FirstName)
                      .Skip(skip).Take(take).ToListAsync(ct);
    }

    public async Task AddAsync(Citizen entity, CancellationToken ct = default)
    { _db.Citizens.Add(entity); await _db.SaveChangesAsync(ct); }

    public async Task UpdateAsync(Citizen entity, CancellationToken ct = default)
    { _db.Citizens.Update(entity); await _db.SaveChangesAsync(ct); }
}
