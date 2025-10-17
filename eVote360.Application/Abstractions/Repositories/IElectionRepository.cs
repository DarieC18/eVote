using EVote360.Domain.Entities.Ballot;
using EVote360.Domain.Entities.Election;

namespace EVote360.Application.Abstractions.Repositories;

public interface IElectionRepository
{
    Task<Election?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Election>> ListAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Election>> ListAsync(int? year, CancellationToken ct = default);
    Task<Election?> GetByYearAsync(int year, CancellationToken ct = default);
    Task<Election?> GetActiveAsync(CancellationToken ct = default);
    Task AddAsync(Election e, CancellationToken ct = default);
    Task UpdateAsync(Election e, CancellationToken ct = default);
    Task DeleteAsync(Election e, CancellationToken ct = default);
    Task<bool> BallotExistsAsync(Guid electionId, Guid positionId, CancellationToken ct = default);
    Task AddBallotAsync(ElectionBallot ballot, CancellationToken ct = default);
    Task<bool> NingunoExistsAsync(Guid electionId, Guid positionId, CancellationToken ct = default);
    Task AddOptionAsync(BallotOption option, CancellationToken ct = default);
    Task<bool> CandidateOptionExistsAsync(Guid electionId, Guid positionId, Guid candidateId, CancellationToken ct = default);

}
