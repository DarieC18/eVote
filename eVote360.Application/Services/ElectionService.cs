using eVote360.Application.Abstractions.Services;
using EVote360.Application.Abstractions.Repositories;
using EVote360.Domain.Entities.Ballot;
using EVote360.Domain.Entities.Election;
using EVote360.Domain.Enums;

namespace EVote360.Application.Services;

public sealed class ElectionService : IElectionService
{
    private readonly IElectionRepository _elections;
    private readonly IPartyRepository _parties;
    private readonly IPositionRepository _positions;

    public ElectionService(
        IElectionRepository elections,
        IPartyRepository parties,
        IPositionRepository positions)
    {
        _elections = elections;
        _parties = parties;
        _positions = positions;
    }

    public async Task<(bool ok, string error, Guid id)> CrearAsync(int year, CancellationToken ct = default)
    {
        var e = new Election($"Elección {year}", year);
        await _elections.AddAsync(e, ct);
        return (true, "", e.Id);
    }
    public async Task<IReadOnlyList<(Guid id, int year, string status)>> ListAsync(CancellationToken ct = default)
    {
        var list = await _elections.ListAsync(ct);
        return list.Select(e => (e.Id, e.Year, e.Status.ToString())).ToList();
    }
    public async Task<(bool ok, string error)> ActivarAsync(Guid electionId, CancellationToken ct = default)
    {
        var e = await _elections.GetByIdAsync(electionId, ct);
        if (e is null) return (false, "Elección no encontrada.");

        if (e.Status == ElectionStatus.Finished)
            return (false, "Esta elección ya ha finalizado. No se puede activar.");

        var activa = await _elections.GetActiveAsync(ct);
        if (activa is not null && activa.Id != electionId)
        {
            return (false, "Ya existe una elección activa. Debe finalizarla antes de activar otra.");
        }

        var partidosActivos = (await _parties.ListAsync(ct)).Where(p => p.IsActive).ToList();
        if (partidosActivos.Count < 2)
            return (false, "Debe haber al menos 2 partidos activos para activar la elección.");

        var puestosActivos = (await _positions.ListAsync(ct)).Where(p => p.IsActive).ToList();
        if (puestosActivos.Count == 0)
            return (false, "Debe haber al menos un puesto activo para activar la elección.");

        var build = await ConstruirBoletaAsync(electionId, ct);
        if (!build.ok) return (false, build.error);

        e.Start();
        await _elections.UpdateAsync(e, ct);
        return (true, "");
    }
    public async Task<(bool ok, string error)> FinalizarAsync(Guid electionId, CancellationToken ct = default)
    {
        var e = await _elections.GetByIdAsync(electionId, ct);
        if (e is null) return (false, "Elección no encontrada.");

        if (e.Status == ElectionStatus.Finished)
        {
            return (false, "La elección ya está finalizada.");
        }

        e.Finish();
        await _elections.UpdateAsync(e, ct);
        return (true, "");
    }

    public async Task<(bool ok, string error)> ConstruirBoletaAsync(Guid electionId, CancellationToken ct = default)
    {
        var e = await _elections.GetByIdAsync(electionId, ct);
        if (e is null) return (false, "Elección no encontrada.");

        var partidosActivos = (await _parties.ListAsync(ct)).Where(p => p.IsActive).ToList();
        if (partidosActivos.Count < 2) return (false, "Debe haber al menos 2 partidos activos.");

        var puestosActivos = (await _positions.ListAsync(ct)).Where(p => p.IsActive).ToList();
        if (puestosActivos.Count == 0) return (false, "Debe haber al menos un puesto activo.");

        foreach (var pos in puestosActivos)
        {
            if (!await _elections.BallotExistsAsync(electionId, pos.Id, ct))
            {
                await _elections.AddBallotAsync(new ElectionBallot(electionId, pos.Id), ct);
            }

            if (!await _elections.NingunoExistsAsync(electionId, pos.Id, ct))
            {
                var ninguno = BallotOption.CreateNinguno(electionId, pos.Id);
                await _elections.AddOptionAsync(ninguno, ct);
            }
        }

        await _elections.UpdateAsync(e, ct);
        return (true, "");
    }
}
