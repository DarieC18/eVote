using EVote360.Application.Abstractions.Repositories;
using EVote360.Domain.Entities.Ballot;
using EVote360.Domain.Entities.Election;
using EVote360.Domain.Enums;
using EVote360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EVote360.Infrastructure.Repositories;

public sealed class ElectionRepository : IElectionRepository
{
    private readonly AppDbContext _db;
    public ElectionRepository(AppDbContext db) => _db = db;

    public Task<Election?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _db.Elections.FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<IReadOnlyList<Election>> ListAsync(CancellationToken ct = default)
        => await _db.Elections.AsNoTracking()
               .OrderByDescending(e => e.Year).ThenBy(e => e.Name)
               .ToListAsync(ct);

    public async Task<IReadOnlyList<Election>> ListAsync(int? year, CancellationToken ct = default)
    {
        var q = _db.Elections.AsNoTracking().AsQueryable();
        if (year.HasValue) q = q.Where(e => e.Year == year.Value);
        return await q.OrderByDescending(e => e.Year).ThenBy(e => e.Name).ToListAsync(ct);
    }

    public Task<Election?> GetByYearAsync(int year, CancellationToken ct = default)
        => _db.Elections.AsNoTracking().FirstOrDefaultAsync(e => e.Year == year, ct);

    public Task<Election?> GetActiveAsync(CancellationToken ct = default)
        => _db.Elections.AsNoTracking()
            .FirstOrDefaultAsync(e => e.Status == ElectionStatus.Active, ct);

    public async Task AddAsync(Election e, CancellationToken ct = default)
    {
        _db.Elections.Add(e);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Election e, CancellationToken ct = default)
    {
        _db.Elections.Update(e);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Election e, CancellationToken ct = default)
    {
        _db.Elections.Remove(e);
        await _db.SaveChangesAsync(ct);
    }

    public Task<bool> BallotExistsAsync(Guid electionId, Guid positionId, CancellationToken ct = default)
        => _db.ElectionBallots.AsNoTracking()
              .AnyAsync(b => b.ElectionId == electionId && b.PositionId == positionId, ct);

    public async Task AddBallotAsync(ElectionBallot ballot, CancellationToken ct = default)
    {
        _db.ElectionBallots.Add(ballot);
        await _db.SaveChangesAsync(ct);
    }

    public Task<bool> NingunoExistsAsync(Guid electionId, Guid positionId, CancellationToken ct = default)
        => _db.BallotOptions.AsNoTracking()
              .AnyAsync(o => o.ElectionId == electionId &&
                             o.PositionId == positionId &&
                             o.IsNinguno, ct);

    public async Task AddOptionAsync(BallotOption option, CancellationToken ct = default)
    {
        _db.BallotOptions.Add(option);
        await _db.SaveChangesAsync(ct);
    }

    public Task<bool> CandidateOptionExistsAsync(Guid electionId, Guid positionId, Guid candidateId, CancellationToken ct = default)
        => _db.BallotOptions.AsNoTracking()
              .AnyAsync(o => o.ElectionId == electionId &&
                             o.PositionId == positionId &&
                             o.CandidateId == candidateId, ct);
}
