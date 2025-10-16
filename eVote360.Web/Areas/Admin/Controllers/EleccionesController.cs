using EVote360.Application.Elecciones;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EVote360.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Administrador")]
public class EleccionesController : Controller
{
    private readonly IElectionService _svc;
    public EleccionesController(IElectionService svc) => _svc = svc;

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var list = await _svc.ListAsync(ct);
        return View(list);
    }

    [HttpPost]
    public async Task<IActionResult> Crear(int year, CancellationToken ct)
    {
        var (ok, err, _) = await _svc.CrearAsync(year, ct);
        TempData["msg"] = ok ? "✅ Elección creada correctamente." : err;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Construir(Guid id, CancellationToken ct)
    {
        var (ok, err) = await _svc.ConstruirBoletaAsync(id, ct);
        TempData["msg"] = ok ? "🗳️ Boleta construida correctamente." : err;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Activar(Guid id, CancellationToken ct)
    {
        var (ok, err) = await _svc.ActivarAsync(id, ct);
        TempData["msg"] = ok ? "✅ Elección activada." : err;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Finalizar(Guid id, CancellationToken ct)
    {
        var (ok, err) = await _svc.FinalizarAsync(id, ct);
        TempData["msg"] = ok ? "🛑 Elección finalizada." : err;
        return RedirectToAction(nameof(Index));
    }
}
