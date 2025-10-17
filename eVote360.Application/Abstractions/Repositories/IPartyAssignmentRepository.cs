using EVote360.Domain.Entities.Assignments;

namespace EVote360.Application.Abstractions.Repositories
{
    public interface IPartyAssignmentRepository
    {
        Task<IReadOnlyList<PartyAssignment>> GetByUserAsync(int usuarioId, CancellationToken ct = default);
        Task AddAsync(PartyAssignment entity, CancellationToken ct = default);
        Task RemoveAsync(PartyAssignment entity, CancellationToken ct = default);
    }
}
