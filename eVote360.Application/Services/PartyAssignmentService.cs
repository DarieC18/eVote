using EVote360.Application.Abstractions.Repositories;
using EVote360.Application.Abstractions.Services;
using EVote360.Application.DTOs.PartyAssignments;
using EVote360.Domain.Entities.Assignments;

public class PartyAssignmentService : IPartyAssignmentService
{
    private readonly IPartyAssignmentRepository _assignments;
    private readonly IUsuarioRepository _users;
    private readonly IPartyRepository _parties;

    public PartyAssignmentService(
        IPartyAssignmentRepository assignments,
        IUsuarioRepository users,
        IPartyRepository parties)
    {
        _assignments = assignments;
        _users = users;
        _parties = parties;
    }

    public async Task<List<PartyAssignmentResponseDto>> ListAsync(int usuarioId, CancellationToken ct = default)
    {
        var list = await _assignments.GetByUserAsync(usuarioId, ct);
        var result = new List<PartyAssignmentResponseDto>();

        foreach (var a in list)
        {
            var u = await _users.GetByIdAsync(a.UsuarioId, ct);
            var p = await _parties.GetByIdAsync(a.PartyId, ct);

            result.Add(new PartyAssignmentResponseDto
            {
                Id = a.Id,
                UsuarioId = a.UsuarioId,
                UsuarioNombre = u is null ? "" : $"{u.Apellido}, {u.Nombre}",
                PartyId = a.PartyId,
                PartyNombre = p?.Name ?? "",
                Activo = a.Activo
            });
        }
        return result;
    }

    public async Task<(bool ok, string? error)> CreateAsync(Guid partyId, int usuarioId, CancellationToken ct = default)
    {
        var user = await _users.GetByIdAsync(usuarioId, ct);
        if (user is null) return (false, "Usuario no existe.");

        var party = await _parties.GetByIdAsync(partyId, ct);
        if (party is null) return (false, "Partido no existe.");

        // evita duplicados simples
        var existentes = await _assignments.GetByUserAsync(usuarioId, ct);
        if (existentes.Any(x => x.PartyId == partyId))
            return (false, "El usuario ya tiene una asignación a este partido.");

        var entity = new PartyAssignment(usuarioId, partyId);
        await _assignments.AddAsync(entity, ct);
        return (true, null);
    }

    public async Task<(bool ok, string? error)> ToggleAsync(Guid id, int usuarioId, CancellationToken ct = default)
    {
        var list = await _assignments.GetByUserAsync(usuarioId, ct);
        var a = list.FirstOrDefault(x => x.Id == id);
        if (a is null) return (false, "Asignación no encontrada.");

        if (a.Activo) a.Desactivar(); else a.Activar();

        await _assignments.RemoveAsync(a, ct);
        await _assignments.AddAsync(a, ct);

        return (true, null);
    }
}
