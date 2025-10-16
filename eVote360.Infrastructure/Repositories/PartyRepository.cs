using EVote360.Application.Abstractions.Repositories;
using EVote360.Domain.Entities.Party;
using EVote360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EVote360.Infrastructure.Repositories;

public sealed class PartyRepository : IPartyRepository
{
    private readonly AppDbContext _db;
    public PartyRepository(AppDbContext db) => _db = db;

    public Task<Party?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _db.Parties.FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<Party?> GetBySiglasAsync(string siglas, CancellationToken ct = default)
        => _db.Parties.FirstOrDefaultAsync(x => x.Siglas == siglas, ct);

    public async Task<IReadOnlyList<Party>> ListAsync(CancellationToken ct = default)
        => await _db.Parties.AsNoTracking().OrderBy(p => p.Name).ToListAsync(ct);

    public async Task AddAsync(Party entity, CancellationToken ct = default)
    { _db.Parties.Add(entity); await _db.SaveChangesAsync(ct); }

    public async Task UpdateAsync(Party entity, CancellationToken ct = default)
    { _db.Parties.Update(entity); await _db.SaveChangesAsync(ct); }
}
