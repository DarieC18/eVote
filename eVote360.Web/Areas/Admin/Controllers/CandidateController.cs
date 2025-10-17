using EVote360.Application.Abstractions.Services;
using EVote360.Application.DTOs.Candidates.Requests;
using EVote360.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EVote360.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Administrador")]
public sealed class CandidatesController : Controller
{
    private readonly ICandidateService _svc;
    private readonly AppDbContext _db;

    public CandidatesController(ICandidateService svc, AppDbContext db)
    {
        _svc = svc;
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Index(Guid? partyId, CancellationToken ct)
    {
        ViewBag.Parties = await _db.Parties.AsNoTracking()
                              .OrderBy(p => p.Name)
                              .Select(p => new { p.Id, p.Name })
                              .ToListAsync(ct);

        if (partyId is null) return View(Array.Empty<object>());

        var list = await _svc.ListByPartyAsync(partyId.Value, ct);
        ViewBag.PartyId = partyId.Value;

        ViewBag.Positions = await _db.Positions.AsNoTracking()
                               .OrderBy(p => p.Name)
                               .Select(p => new { p.Id, p.Name })
                               .ToListAsync(ct);

        return View(list);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CandidateCreateRequestDto request, CancellationToken ct)
    {
        var (ok, err, _) = await _svc.CreateAsync(request, ct);
        TempData[ok ? "Ok" : "Error"] = ok ? "Candidato creado." : err;
        return RedirectToAction(nameof(Index), new { partyId = request.PartyId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(CandidateUpdateRequestDto request, CancellationToken ct)
    {
        var (ok, err) = await _svc.UpdateAsync(request, ct);
        TempData[ok ? "Ok" : "Error"] = ok ? "Candidato actualizado." : err;
        return RedirectToAction(nameof(Index), new { partyId = request.PartyId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, Guid partyId, CancellationToken ct)
    {
        var (ok, err) = await _svc.DeleteAsync(id, ct);
        TempData[ok ? "Ok" : "Error"] = ok ? "Candidato eliminado." : err;
        return RedirectToAction(nameof(Index), new { partyId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Assign(Guid candidateId, Guid partyId, Guid positionId, CancellationToken ct)
    {
        var (ok, err) = await _svc.AssignToPositionAsync(candidateId, positionId, ct);
        TempData[ok ? "Ok" : "Error"] = ok ? "Asignado a puesto en la elección activa." : err;
        return RedirectToAction(nameof(Index), new { partyId });
    }
}
