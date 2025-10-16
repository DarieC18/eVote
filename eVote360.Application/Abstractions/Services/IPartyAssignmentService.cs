using EVote360.Application.DTOs.PartyAssignments;

namespace EVote360.Application.Abstractions.Services;

public interface IPartyAssignmentService
{
    Task<List<PartyAssignmentResponseDto>> ListAsync(int usuarioId, CancellationToken ct = default);
    Task<(bool ok, string? error)> CreateAsync(Guid partyId, int usuarioId, CancellationToken ct = default);
    Task<(bool ok, string? error)> ToggleAsync(Guid id, int usuarioId, CancellationToken ct = default);
}
