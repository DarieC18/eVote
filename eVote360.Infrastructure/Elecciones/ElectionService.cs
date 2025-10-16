using EVote360.Application.Elecciones;
using EVote360.Domain.Entities.Election;
using EVote360.Domain.Entities.Ballot;
using EVote360.Domain.Enums;
using EVote360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EVote360.Infrastructure.Elecciones;

public class ElectionService : IElectionService
{
    private readonly AppDbContext _ctx;
    public ElectionService(AppDbContext ctx) => _ctx = ctx;

    public async Task<(bool ok, string error, Guid id)> CrearAsync(int year, CancellationToken ct = default)
    {
        if (await _ctx.Elections.AnyAsync(e => e.Year == year, ct))
            return (false, "Ya existe una elección para ese año.", Guid.Empty);

        var e = new Election($"Elecciones {year}", year);
        _ctx.Elections.Add(e);
        await _ctx.SaveChangesAsync(ct);
        return (true, "", e.Id);
    }

    public async Task<(bool ok, string error)> ActivarAsync(Guid electionId, CancellationToken ct = default)
    {
        var e = await _ctx.Elections.FirstOrDefaultAsync(x => x.Id == electionId, ct);
        if (e is null) return (false, "Elección no encontrada.");

        // Solo puede haber una activa
        if (await _ctx.Elections.AnyAsync(x => x.Status == ElectionStatus.Active && x.Id != electionId, ct))
            return (false, "Ya hay una elección activa.");

        var tieneBoleta = await _ctx.ElectionBallots.AnyAsync(b => b.ElectionId == electionId, ct);
        if (!tieneBoleta) return (false, "No hay boleta configurada.");

        try
        {
            e.Start();
        }
        catch (InvalidOperationException ex)
        {
            return (false, ex.Message);
        }

        await _ctx.SaveChangesAsync(ct);
        return (true, "");
    }

    public async Task<(bool ok, string error)> FinalizarAsync(Guid electionId, CancellationToken ct = default)
    {
        var e = await _ctx.Elections.FirstOrDefaultAsync(x => x.Id == electionId, ct);
        if (e is null) return (false, "Elección no encontrada.");

        try
        {
            e.Finish();
        }
        catch (InvalidOperationException ex)
        {
            return (false, ex.Message);
        }

        await _ctx.SaveChangesAsync(ct);
        return (true, "");
    }

    public async Task<(bool ok, string error)> ConstruirBoletaAsync(Guid electionId, CancellationToken ct = default)
    {
        var oldBallots = await _ctx.ElectionBallots
            .Where(b => b.ElectionId == electionId)
            .ToListAsync(ct);

        if (oldBallots.Count > 0)
        {
            var ballotIds = oldBallots.Select(b => b.Id).ToList();
            var oldOptions = await _ctx.BallotOptions
                .Where(o => ballotIds.Contains(o.ElectionBallotId))
                .ToListAsync(ct);

            _ctx.BallotOptions.RemoveRange(oldOptions);
            _ctx.ElectionBallots.RemoveRange(oldBallots);
            await _ctx.SaveChangesAsync(ct);
        }

        var puestos = await _ctx.Positions
            .AsNoTracking()
            .Select(p => new { p.Id })
            .ToListAsync(ct);

        foreach (var p in puestos)
        {
            var eb = new ElectionBallot(electionId, p.Id);
            _ctx.ElectionBallots.Add(eb);
            await _ctx.SaveChangesAsync(ct);

            var candidateIds = await (
                from cnd in _ctx.Candidates
                join candPos in _ctx.Set<EVote360.Domain.Entities.Candidate.Candidatura>() on cnd.Id equals candPos.CandidateId
                where candPos.PositionId == p.Id
                select cnd.Id
            ).Distinct().ToListAsync(ct);

            foreach (var cid in candidateIds)
                _ctx.BallotOptions.Add(BallotOption.ForCandidate(eb.Id, cid));
            await _ctx.SaveChangesAsync(ct);
        }

        return (true, "");
    }
    public async Task<IReadOnlyList<(Guid id, int year, string status)>> ListAsync(CancellationToken ct = default)
    {
        var data = await _ctx.Elections.AsNoTracking()
            .OrderByDescending(x => x.Year)
            .Select(e => new { e.Id, e.Year, e.Status })
            .ToListAsync(ct);

        return data.Select(x => (x.Id, x.Year, x.Status.ToString())).ToList();
    }
}
