using EVote360.Application.Votacion;
using EVote360.Domain.Entities.Election;
using EVote360.Domain.Enums;
using EVote360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EVote360.Infrastructure.Votacion
{
    public class VotacionService : IVotacionService
    {
        private readonly AppDbContext _ctx;
        public VotacionService(AppDbContext ctx) => _ctx = ctx;

        public async Task<(bool ok, string error)> ValidarElegibilidadAsync(string nationalId, CancellationToken ct = default)
        {
            // NationalId es VO -> usa .Value
            var citizen = await _ctx.Citizens.FirstOrDefaultAsync(c => c.NationalId.Value == nationalId && c.IsActive, ct);
            if (citizen is null) return (false, "Cédula no válida o ciudadano inactivo.");
            return (true, "");
        }

        public async Task<(bool ok, string error, Guid electionId)> ObtenerEleccionActivaAsync(CancellationToken ct = default)
        {
            var e = await _ctx.Elections.FirstOrDefaultAsync(x => x.Status == ElectionStatus.Active, ct);
            return e is null ? (false, "No hay proceso electoral activo.", Guid.Empty) : (true, "", e.Id);
        }

        public async Task<(bool ok, string error)> RegistrarVotoAsync(Guid electionId, Guid citizenId, Guid positionId, Guid? candidateId, CancellationToken ct = default)
        {
            var opcion = await (
                from bo in _ctx.BallotOptions
                join b in _ctx.ElectionBallots on bo.ElectionBallotId equals b.Id
                where b.ElectionId == electionId
                      && b.PositionId == positionId
                      && ((candidateId == null && bo.IsNinguno) || (candidateId != null && bo.CandidateId == candidateId))
                select new { BallotOptionId = bo.Id }
            ).FirstOrDefaultAsync(ct);

            if (opcion is null)
                return (false, "Opción no válida para la boleta actual.");

            var vote = await _ctx.Votes.FirstOrDefaultAsync(v => v.ElectionId == electionId && v.CitizenId == citizenId, ct);
            if (vote is null)
            {
                vote = new Domain.Entities.Vote.Vote(electionId, citizenId, Guid.NewGuid().ToString("N"));
                await _ctx.Votes.AddAsync(vote, ct);
                await _ctx.SaveChangesAsync(ct);
            }

            var yaVotoEsePuesto = await (
                from vi in _ctx.VoteItems
                where vi.VoteId == vote.Id
                join bo2 in _ctx.BallotOptions on vi.BallotOptionId equals bo2.Id
                join b2 in _ctx.ElectionBallots on bo2.ElectionBallotId equals b2.Id
                where b2.PositionId == positionId
                select vi
            ).AnyAsync(ct);

            if (yaVotoEsePuesto)
                return (false, "Ya registraste un voto para este puesto.");

            var item = new Domain.Entities.Vote.VoteItem(vote.Id, opcion.BallotOptionId);
            await _ctx.VoteItems.AddAsync(item, ct);
            await _ctx.SaveChangesAsync(ct);

            return (true, "");
        }

        public async Task<bool> CiudadanoCompletoVotosAsync(Guid electionId, Guid citizenId, CancellationToken ct = default)
        {
            // total de puestos en la boleta activa
            var puestos = await _ctx.ElectionBallots
                .Where(b => b.ElectionId == electionId)
                .Select(b => b.PositionId)
                .Distinct()
                .CountAsync(ct);

            if (puestos == 0) return false;

            // votos del ciudadano por puesto (via joins)
            var votosHechos = await (
                from vi in _ctx.VoteItems
                join v in _ctx.Votes on vi.VoteId equals v.Id
                where v.ElectionId == electionId && v.CitizenId == citizenId
                join bo in _ctx.BallotOptions on vi.BallotOptionId equals bo.Id
                join b in _ctx.ElectionBallots on bo.ElectionBallotId equals b.Id
                select b.PositionId
            ).Distinct().CountAsync(ct);

            return votosHechos >= puestos;
        }

        public async Task<(string ciudadanoNombre, string ciudadanoEmail, IReadOnlyList<(string puesto, string opcion)> detalle)>
            ResumenVotoAsync(Guid electionId, Guid citizenId, CancellationToken ct = default)
        {
            var citizen = await _ctx.Citizens.FirstAsync(c => c.Id == citizenId, ct);

            var detalle = await (
                from vi in _ctx.VoteItems
                join v in _ctx.Votes on vi.VoteId equals v.Id
                where v.ElectionId == electionId && v.CitizenId == citizenId
                join bo in _ctx.BallotOptions on vi.BallotOptionId equals bo.Id
                join b in _ctx.ElectionBallots on bo.ElectionBallotId equals b.Id
                join p in _ctx.Positions on b.PositionId equals p.Id
                join cnd in _ctx.Candidates on bo.CandidateId equals cnd.Id into candLeft
                from cnd in candLeft.DefaultIfEmpty()
                select new
                {
                    Puesto = p.Name,
                    Opcion = bo.IsNinguno ? "Ninguno" : (cnd.FirstName + " " + cnd.LastName)
                }
            ).ToListAsync(ct);

            var nombre = $"{citizen.FirstName} {citizen.LastName}";
            var email = citizen.Email?.Value ?? ""; // Email es VO

            return (nombre, email, detalle.Select(x => (x.Puesto, x.Opcion)).ToList());
        }

        public async Task<bool> YaEjecutoVotoAsync(Guid electionId, Guid citizenId, CancellationToken ct = default)
        {
            return await CiudadanoCompletoVotosAsync(electionId, citizenId, ct);
        }
        public async Task<IReadOnlyList<(Guid positionId, string positionName, int partidos, int candidatos)>> ConteoPorPuestoAsync(Guid electionId, CancellationToken ct = default)
        {
            var q = from eb in _ctx.ElectionBallots
                    where eb.ElectionId == electionId
                    join p in _ctx.Positions on eb.PositionId equals p.Id
                    join bo in _ctx.BallotOptions on eb.Id equals bo.ElectionBallotId
                    join cand in _ctx.Candidates on bo.CandidateId equals cand.Id into candLeft
                    from cand in candLeft.DefaultIfEmpty()
                    group new { bo, cand } by new { eb.PositionId, p.Name } into g
                    select new
                    {
                        g.Key.PositionId,
                        PositionName = g.Key.Name,
                        Partidos = g.Where(x => x.cand != null).Select(x => x.cand!.PartyId).Distinct().Count(),
                        Candidatos = g.Where(x => x.bo.CandidateId != null).Select(x => x.bo.CandidateId!.Value).Distinct().Count()
                    };

            var data = await q.ToListAsync(ct);
            return data.Select(x => (x.PositionId, x.PositionName, x.Partidos, x.Candidatos)).ToList();
        }

        public async Task<IReadOnlyList<(Guid positionId, string positionName)>> PuestosFaltantesAsync(Guid electionId, Guid citizenId, CancellationToken ct = default)
        {
            var puestos = from eb in _ctx.ElectionBallots
                          where eb.ElectionId == electionId
                          join p in _ctx.Positions on eb.PositionId equals p.Id
                          select new { eb.PositionId, p.Name };

            var votados = from vi in _ctx.VoteItems
                          join v in _ctx.Votes on vi.VoteId equals v.Id
                          where v.ElectionId == electionId && v.CitizenId == citizenId
                          join bo in _ctx.BallotOptions on vi.BallotOptionId equals bo.Id
                          join eb in _ctx.ElectionBallots on bo.ElectionBallotId equals eb.Id
                          select eb.PositionId;

            var faltantes = await puestos
                .GroupBy(x => new { x.PositionId, x.Name })
                .Select(g => new { g.Key.PositionId, g.Key.Name })
                .Where(x => !votados.Contains(x.PositionId))
                .ToListAsync(ct);

            return faltantes.Select(x => (x.PositionId, x.Name)).ToList();
        }

    }
}
