using eVote360.Application.Abstractions.Services;
using EVote360.Application.Abstractions.Repositories;
using EVote360.Domain.Entities.Election;

namespace EVote360.Application.Services;

public class ElectionService : IElectionService
{
    private readonly IElectionRepository _elections;
    private readonly IPartyRepository _parties;
    private readonly IPositionRepository _positions;

    public ElectionService(IElectionRepository elections)
        => _elections = elections;

    public async Task<(bool ok, string error, Guid id)> CrearAsync(int year, CancellationToken ct = default)
    {
        var exists = await _elections.GetByYearAsync(year, ct);
        if (exists is not null)
            return (false, "Ya existe una elección para ese año.", Guid.Empty);

        var e = new Election($"Elecciones {year}", year);
        await _elections.AddAsync(e, ct);
        return (true, "", e.Id);
    }

    public async Task<(bool ok, string error)> ActivarAsync(Guid electionId, CancellationToken ct = default)
    {
        var e = await _elections.GetByIdAsync(electionId, ct);
        if (e is null) return (false, "Elección no encontrada.");

        var activa = await _elections.GetActiveAsync(ct);
        if (activa is not null && activa.Id != electionId)
            return (false, "Ya hay una elección activa.");

        try { e.Start(); }
        catch (InvalidOperationException ex) { return (false, ex.Message); }

        await _elections.UpdateAsync(e, ct);
        return (true, "");
    }

    public async Task<(bool ok, string error)> FinalizarAsync(Guid electionId, CancellationToken ct = default)
    {
        var e = await _elections.GetByIdAsync(electionId, ct);
        if (e is null) return (false, "Elección no encontrada.");

        try { e.Finish(); }
        catch (InvalidOperationException ex) { return (false, ex.Message); }

        await _elections.UpdateAsync(e, ct);
        return (true, "");
    }

    public async Task<IReadOnlyList<(Guid id, int year, string status)>> ListAsync(CancellationToken ct = default)
    {
        var list = await _elections.ListAsync(null, ct);
        return list
            .OrderByDescending(x => x.Year)
            .Select(x => (x.Id, x.Year, x.Status.ToString()))
            .ToList();
    }
    public async Task<(bool ok, string error)> ConstruirBoletaAsync(Guid electionId, CancellationToken ct = default)
    {
        var e = await _elections.GetByIdAsync(electionId, ct);
        if (e is null) return (false, "Elección no encontrada.");

        var partidos = await _parties.ListAsync(ct);
        if (partidos.Count(p => p.IsActive) < 2)
            return (false, "Debe haber al menos 2 partidos activos.");

        var puestos = await _positions.ListAsync(ct);
        if (!puestos.Any(p => p.IsActive))
            return (false, "Debe haber al menos un puesto activo.");

        await _elections.UpdateAsync(e, ct);
        return (true, "");
    }

}
