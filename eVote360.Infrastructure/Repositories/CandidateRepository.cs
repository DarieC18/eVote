using EVote360.Application.Abstractions.Repositories;
using EVote360.Domain.Entities;
using EVote360.Domain.Entities.Candidate;
using EVote360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EVote360.Infrastructure.Repositories;

public sealed class CandidateRepository : ICandidateRepository
{
    private readonly AppDbContext _db;
    public CandidateRepository(AppDbContext db) => _db = db;

    public Task<Candidate?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _db.Candidates.FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<IReadOnlyList<Candidate>> ListByPartyAsync(Guid partyId, CancellationToken ct = default)
        => await _db.Candidates.AsNoTracking()
               .Where(c => c.PartyId == partyId)
               .OrderBy(c => c.LastName).ThenBy(c => c.FirstName)
               .ToListAsync(ct);

    public async Task AddAsync(Candidate entity, CancellationToken ct = default)
    {
        _db.Candidates.Add(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Candidate entity, CancellationToken ct = default)
    {
        _db.Candidates.Update(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Candidate entity, CancellationToken ct = default)
    {
        _db.Candidates.Remove(entity);
        await _db.SaveChangesAsync(ct);
    }
}
