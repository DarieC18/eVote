using eVote360.Application.Abstractions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EVote360.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Administrador")]
public sealed class EleccionesController : Controller
{
    private readonly IElectionService _svc;

    public EleccionesController(IElectionService svc) => _svc = svc;

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var list = await _svc.ListAsync(ct);
        return View(list);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(int year, CancellationToken ct)
    {
        var (ok, err, _) = await _svc.CrearAsync(year, ct);
        TempData[ok ? "Ok" : "Error"] = ok ? "Elección creada." : err;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ConstruirBoleta(Guid id, CancellationToken ct)
    {
        var (ok, err) = await _svc.ConstruirBoletaAsync(id, ct);
        TempData[ok ? "Ok" : "Error"] = ok ? "Boletas generadas/aseguradas." : err;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Activar(Guid id, CancellationToken ct)
    {
        var (ok, err) = await _svc.ActivarAsync(id, ct);
        TempData[ok ? "Ok" : "Error"] = ok ? "Elección activada." : err;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Finalizar(Guid id, CancellationToken ct)
    {
        var (ok, err) = await _svc.FinalizarAsync(id, ct);
        TempData[ok ? "Ok" : "Error"] = ok ? "Elección finalizada." : err;
        return RedirectToAction(nameof(Index));
    }
}
