using EVote360.Application.Abstractions.Repositories;
using EVote360.Domain.Entities.Vote;
using EVote360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EVote360.Infrastructure.Repositories;

public sealed class VoteRepository : IVoteRepository
{
    private readonly AppDbContext _db;
    public VoteRepository(AppDbContext db) => _db = db;

    public Task<Vote?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _db.Votes.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task AddAsync(Vote entity, CancellationToken ct = default)
    { _db.Votes.Add(entity); await _db.SaveChangesAsync(ct); }

    public Task<int> CountByCitizenAsync(Guid citizenId, CancellationToken ct = default)
        => _db.Votes.AsNoTracking().CountAsync(v => v.CitizenId == citizenId, ct);

    public Task<int> CountByCitizenInElectionAsync(Guid electionId, Guid citizenId, CancellationToken ct = default)
        => _db.Votes.AsNoTracking().CountAsync(v => v.ElectionId == electionId && v.CitizenId == citizenId, ct);

    public Task<int> CountByElectionAsync(Guid electionId, CancellationToken ct = default)
        => _db.Votes.AsNoTracking().CountAsync(v => v.ElectionId == electionId, ct);
}
