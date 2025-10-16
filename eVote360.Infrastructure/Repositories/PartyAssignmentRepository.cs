using EVote360.Domain.Entities.Assignments;
using EVote360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EVote360.Infrastructure.Repositories;

public sealed class PartyAssignmentRepository : IPartyAssignmentRepository
{
    private readonly AppDbContext _db;
    public PartyAssignmentRepository(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<PartyAssignment>> GetByUserAsync(int usuarioId, CancellationToken ct = default)
        => await _db.PartyAssignments.AsNoTracking()
             .Where(x => x.UsuarioId == usuarioId).ToListAsync(ct);

    public async Task AddAsync(PartyAssignment entity, CancellationToken ct = default)
    { _db.PartyAssignments.Add(entity); await _db.SaveChangesAsync(ct); }

    public async Task RemoveAsync(PartyAssignment entity, CancellationToken ct = default)
    { _db.PartyAssignments.Remove(entity); await _db.SaveChangesAsync(ct); }
}
