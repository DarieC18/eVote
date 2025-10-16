using EVote360.Application.Abstractions.Repositories;
using EVote360.Application.Abstractions.Services;
using EVote360.Application.DTOs.PartyAssignments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EVote360.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Administrador")]
public class AsignacionesController : Controller
{
    private readonly IPartyAssignmentService _service;
    private readonly IUsuarioRepository _usuarios;
    private readonly IPartyRepository _parties;

    public AsignacionesController(
        IPartyAssignmentService service,
        IUsuarioRepository usuarios,
        IPartyRepository parties)
    {
        _service = service;
        _usuarios = usuarios;
        _parties = parties;
    }

    [HttpGet]
    public async Task<IActionResult> Dirigentes(int? usuarioId, CancellationToken ct)
    {
        var dirigentes = await _usuarios.ListAsync(ct);
        var dirOps = dirigentes
            .Where(u => u.Rol == "Dirigente")
            .OrderBy(u => u.Apellido).ThenBy(u => u.Nombre)
            .Select(u => new SelectListItem($"{u.Apellido}, {u.Nombre}", u.Id.ToString()))
            .ToList();

        var selectedId = usuarioId ?? (dirOps.Count > 0 ? int.Parse(dirOps[0].Value) : 0);

        ViewBag.Dirigentes = dirOps;
        ViewBag.SelectedUsuarioId = selectedId;

        var parties = await _parties.ListAsync(ct);
        ViewBag.Partidos = parties
            .OrderBy(p => p.Name)
            .Select(p => new SelectListItem(p.Name, p.Id.ToString()))
            .ToList();

        var list = selectedId == 0
            ? new List<PartyAssignmentResponseDto>()
            : await _service.ListAsync(selectedId, ct);

        return View(list);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(PartyAssignmentCreateDto dto, CancellationToken ct)
    {
        var (ok, error) = await _service.CreateAsync(dto.PartyId, dto.UsuarioId, ct);
        TempData[ok ? "Ok" : "Error"] = ok ? "Asignación creada." : error;
        return RedirectToAction(nameof(Dirigentes), new { usuarioId = dto.UsuarioId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Toggle(Guid id, int usuarioId, CancellationToken ct)
    {
        var (ok, error) = await _service.ToggleAsync(id, usuarioId, ct);
        TempData[ok ? "Ok" : "Error"] = ok ? "Estado actualizado." : error;
        return RedirectToAction(nameof(Dirigentes), new { usuarioId });
    }
}
