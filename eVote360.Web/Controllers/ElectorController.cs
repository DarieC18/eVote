using eVote360.Application.Abstractions.Services;
using EVote360.Application.Common.Notifications;
using EVote360.Application.Common.Ocr;
using EVote360.Infrastructure.Persistence;
using EVote360.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EVote360.Web.Controllers
{
    public class ElectorController : Controller
    {
        private readonly AppDbContext _ctx;
        private readonly IOcrService _ocr;
        private readonly IVotacionService _votacion;
        private readonly IEmailService _email;

        public ElectorController(AppDbContext ctx, IOcrService ocr, IVotacionService votacion, IEmailService email)
        {
            _ctx = ctx;
            _ocr = ocr;
            _votacion = votacion;
            _email = email;
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.ElectorStep = 1;
            return View(new ElectorIdVm());
        }

        [HttpPost]
        public async Task<IActionResult> Index(ElectorIdVm vm, CancellationToken ct)
        {
            ViewBag.ElectorStep = 1;

            if (string.IsNullOrWhiteSpace(vm.NationalId))
            {
                ModelState.AddModelError("", "Debe ingresar su cédula.");
                return View(vm);
            }

            var (okE, errE, electionId) = await _votacion.ObtenerEleccionActivaAsync(ct);
            if (!okE) return View("Mensaje", "No hay ningún proceso electoral en estos momentos");

            var citizen = await _ctx.Citizens.FirstOrDefaultAsync(c => c.NationalId.Value == vm.NationalId, ct);
            if (citizen is null)
            {
                ModelState.AddModelError("", "Cédula no válida.");
                return View(vm);
            }
            if (!citizen.IsActive)
                return View("Mensaje", "El ciudadano está inactivo.");

            var ya = await _votacion.YaEjecutoVotoAsync(electionId, citizen.Id, ct);
            if (ya) return View("Mensaje", "Ya ha ejercido su derecho al voto.");

            TempData["NationalId"] = vm.NationalId;
            return RedirectToAction(nameof(ValidarIdentidad));
        }

        [HttpGet]
        public IActionResult ValidarIdentidad()
        {
            ViewBag.ElectorStep = 2;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ValidarIdentidad(IFormFile? documentoFrontal, CancellationToken ct)
        {
            ViewBag.ElectorStep = 2;

            if (documentoFrontal == null || documentoFrontal.Length == 0)
            {
                ModelState.AddModelError("", "Debe subir una imagen del documento.");
                return View();
            }

            var nationalId = TempData["NationalId"] as string;
            if (string.IsNullOrWhiteSpace(nationalId))
                return RedirectToAction(nameof(Index));

            using var stream = documentoFrontal.OpenReadStream();
            var extraida = await _ocr.ExtractNationalIdAsync(stream, ct);

            if (!string.Equals(extraida, nationalId, StringComparison.Ordinal))
            {
                ModelState.AddModelError("", "La cédula no coincide con el documento.");
                return View();
            }

            TempData["NationalId"] = nationalId;
            return RedirectToAction(nameof(Boleta));
        }

        [HttpGet]
        public async Task<IActionResult> Boleta(CancellationToken ct)
        {
            ViewBag.ElectorStep = 3;

            var nationalId = TempData["NationalId"] as string;
            if (string.IsNullOrWhiteSpace(nationalId))
                return RedirectToAction(nameof(Index));

            TempData["NationalId"] = nationalId;

            var (okE, errE, electionId) = await _votacion.ObtenerEleccionActivaAsync(ct);
            if (!okE) return View("Mensaje", errE);

            var citizen = await _ctx.Citizens
                .AsNoTracking()
                .FirstAsync(c => c.NationalId.Value == nationalId, ct);

            var puestos = await (
                from eb in _ctx.ElectionBallots.AsNoTracking()
                join p in _ctx.Positions.AsNoTracking() on eb.PositionId equals p.Id
                where eb.ElectionId == electionId
                select new { eb.PositionId, p.Name }
            ).Distinct().ToListAsync(ct);

            var votados = await (
                from vi in _ctx.VoteItems.AsNoTracking()
                join v in _ctx.Votes.AsNoTracking() on vi.VoteId equals v.Id
                join bo in _ctx.BallotOptions.AsNoTracking() on vi.BallotOptionId equals bo.Id
                join eb in _ctx.ElectionBallots.AsNoTracking() on bo.ElectionBallotId equals eb.Id
                where v.ElectionId == electionId && v.CitizenId == citizen.Id
                select eb.PositionId
            ).Distinct().ToListAsync(ct);

            var conteos = await (
                from eb in _ctx.ElectionBallots.AsNoTracking()
                join bo in _ctx.BallotOptions.AsNoTracking() on eb.Id equals bo.ElectionBallotId
                join cnd in _ctx.Candidates.AsNoTracking() on bo.CandidateId equals cnd.Id into candLeft
                from cnd in candLeft.DefaultIfEmpty()
                where eb.ElectionId == electionId
                group new { bo, cnd } by eb.PositionId into g
                select new
                {
                    PositionId = g.Key,
                    Partidos = g.Where(x => x.cnd != null).Select(x => x.cnd!.PartyId).Distinct().Count(),
                    Candidatos = g.Where(x => x.bo.CandidateId != null).Select(x => x.bo.CandidateId!.Value).Distinct().Count()
                }
            ).ToListAsync(ct);

            var conteoDict = conteos.ToDictionary(x => x.PositionId);

            var model = new BoletaVm
            {
                ElectionId = electionId,
                CitizenId = citizen.Id,
                Puestos = puestos.Select(p => new EVote360.Web.Models.PuestoVm
                {
                    PositionId = p.PositionId,
                    PositionName = p.Name,
                    YaVotado = votados.Contains(p.PositionId),
                    Partidos = conteoDict.TryGetValue(p.PositionId, out var c) ? c.Partidos : 0,
                    Candidatos = conteoDict.TryGetValue(p.PositionId, out var c2) ? c2.Candidatos : 0
                }).ToList()
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Opciones(Guid electionId, Guid citizenId, Guid positionId, CancellationToken ct)
        {
            ViewBag.ElectorStep = 4;

            var opciones = await (
                from bo in _ctx.BallotOptions
                join eb in _ctx.ElectionBallots on bo.ElectionBallotId equals eb.Id
                where eb.ElectionId == electionId && eb.PositionId == positionId
                join c in _ctx.Candidates on bo.CandidateId equals c.Id into candLeft
                from c in candLeft.DefaultIfEmpty()
                select new OpcionVm
                {
                    CandidateId = bo.CandidateId,
                    Display = bo.IsNinguno ? "Ninguno" : (c.FirstName + " " + c.LastName)
                }
            ).ToListAsync(ct);

            var vm = new OpcionesVm
            {
                ElectionId = electionId,
                CitizenId = citizenId,
                PositionId = positionId,
                Opciones = opciones
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Votar(OpcionesVm vm, CancellationToken ct)
        {
            ViewBag.ElectorStep = 4;

            var (ok, error) = await _votacion.RegistrarVotoAsync(vm.ElectionId, vm.CitizenId, vm.PositionId, vm.CandidateId, ct);
            if (!ok)
            {
                ModelState.AddModelError("", error);
                return await Opciones(vm.ElectionId, vm.CitizenId, vm.PositionId, ct);
            }

            return RedirectToAction(nameof(Boleta));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Finalizar(CancellationToken ct)
        {
            ViewBag.ElectorStep = 5;

            var nationalId = TempData["NationalId"] as string;
            if (string.IsNullOrWhiteSpace(nationalId))
                return RedirectToAction(nameof(Index));

            TempData["NationalId"] = nationalId;

            var (okE, errE, electionId) = await _votacion.ObtenerEleccionActivaAsync(ct);
            if (!okE) return View("Mensaje", errE);

            var citizen = await _ctx.Citizens.FirstAsync(c => c.NationalId.Value == nationalId, ct);
            var completo = await _votacion.CiudadanoCompletoVotosAsync(electionId, citizen.Id, ct);
            if (!completo) return View("Mensaje", "Aún faltan puestos por votar.");

            var (nombre, email, detalle) = await _votacion.ResumenVotoAsync(electionId, citizen.Id, ct);

            if (!string.IsNullOrWhiteSpace(email))
            {
                var cuerpo = $"<h3>Resumen de tu voto</h3><p>{nombre}</p><ul>" +
                             string.Concat(detalle.Select(d => $"<li><b>{d.puesto}:</b> {d.opcion}</li>")) +
                             "</ul>";
                await _email.SendAsync(email, "Confirmación de voto - eVote360", cuerpo, ct);
            }

            return View("Mensaje", "¡Gracias! Tu voto ha sido registrado. Se envió un resumen a tu correo (si estaba disponible).");
        }
        public IActionResult Mensaje(string msg)
        {
            ViewBag.ElectorStep = 0;
            return View(model: msg);
        }
    }
}
