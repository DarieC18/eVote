using AutoMapper;
using EVote360.Application.Abstractions.Services;
using EVote360.Application.DTOs.Request;
using EVote360.Application.ViewModels.Position;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EVote360.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Administrador")]
public class PositionsController : Controller
{
    private readonly IPositionService _service;
    private readonly IMapper _mapper;

    public PositionsController(IPositionService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    public async Task<IActionResult> Index()
    {
        var res = await _service.GetListAsync();
        var vm = res.Succeeded ? _mapper.Map<IEnumerable<PositionListVM>>(res.Data!)
                                : Enumerable.Empty<PositionListVM>();
        if (!res.Succeeded) TempData["Error"] = res.Error;
        return View(vm);
    }

    public IActionResult Create() => View(new PositionFormVM());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PositionFormVM vm)
    {
        if (!ModelState.IsValid) return View(vm);
        var res = await _service.CreateAsync(_mapper.Map<PositionCreateRequest>(vm));
        if (!res.Succeeded) { ModelState.AddModelError(string.Empty, res.Error!); return View(vm); }
        TempData["Success"] = "Puesto creado.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var res = await _service.GetByIdAsync(id);
        if (!res.Succeeded || res.Data is null) { TempData["Error"] = res.Error ?? "No encontrado."; return RedirectToAction(nameof(Index)); }
        ViewBag.PositionId = id;
        return View(new PositionFormVM { Name = res.Data.Name });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, PositionFormVM vm)
    {
        if (!ModelState.IsValid) { ViewBag.PositionId = id; return View(vm); }
        var res = await _service.UpdateAsync(id, _mapper.Map<PositionUpdateRequest>(vm));
        if (!res.Succeeded) { ModelState.AddModelError(string.Empty, res.Error!); ViewBag.PositionId = id; return View(vm); }
        TempData["Success"] = "Puesto actualizado.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Toggle(Guid id)
    {
        var res = await _service.ToggleActiveAsync(id);
        TempData[res.Succeeded ? "Success" : "Error"] = res.Succeeded ? "Estado actualizado." : res.Error;
        return RedirectToAction(nameof(Index));
    }
}
