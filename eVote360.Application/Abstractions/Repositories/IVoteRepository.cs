using EVote360.Domain.Entities.Vote;

namespace EVote360.Application.Abstractions.Repositories;

public interface IVoteRepository
{
    Task<Vote?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Vote entity, CancellationToken ct = default);
    Task<int> CountByCitizenAsync(Guid citizenId, CancellationToken ct = default);
    Task<int> CountByCitizenInElectionAsync(Guid electionId, Guid citizenId, CancellationToken ct = default);
    Task<int> CountByElectionAsync(Guid electionId, CancellationToken ct = default);
}
