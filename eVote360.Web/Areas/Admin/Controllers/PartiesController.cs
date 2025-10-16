using AutoMapper;
using EVote360.Application.Abstractions.Services;
using EVote360.Application.DTOs.Request;
using EVote360.Application.ViewModels.Party;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EVote360.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Administrador")]
public class PartiesController : Controller
{
    private readonly IPartyService _service;
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _env;

    public PartiesController(IPartyService service, IMapper mapper, IWebHostEnvironment env)
    {
        _service = service; _mapper = mapper; _env = env;
    }

    public async Task<IActionResult> Index()
    {
        var res = await _service.GetListAsync();
        return View(res.Succeeded ? res.Data! : Enumerable.Empty<object>());
    }

    public IActionResult Create() => View(new PartyFormVM());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PartyFormVM vm, IFormFile? logo)
    {
        if (!ModelState.IsValid) return View(vm);

        var req = _mapper.Map<PartyCreateRequest>(vm);
        req.LogoPath = await SaveLogoAsync(logo);

        var res = await _service.CreateAsync(req);
        if (!res.Succeeded) { ModelState.AddModelError("", res.Error!); return View(vm); }

        TempData["Success"] = "Partido creado.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var res = await _service.GetByIdAsync(id);
        if (!res.Succeeded) return NotFound();

        var vm = new PartyFormVM
        {
            Name = res.Data!.Name,
            Siglas = res.Data.Acronym,
            Description = res.Data.Description,
            LogoPath = res.Data.LogoPath
        };
        ViewBag.Id = id;
        return View(vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, PartyFormVM vm, IFormFile? logo)
    {
        if (!ModelState.IsValid) { ViewBag.Id = id; return View(vm); }

        var req = _mapper.Map<PartyUpdateRequest>(vm);
        var newLogo = await SaveLogoAsync(logo);
        if (!string.IsNullOrWhiteSpace(newLogo)) req.LogoPath = newLogo; // solo si suben nuevo

        var res = await _service.UpdateAsync(id, req);
        if (!res.Succeeded) { ModelState.AddModelError("", res.Error!); ViewBag.Id = id; return View(vm); }

        TempData["Success"] = "Partido actualizado.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Toggle(Guid id)
    {
        var res = await _service.ToggleActiveAsync(id);
        TempData[res.Succeeded ? "Success" : "Error"] = res.Succeeded ? "Estado cambiado." : res.Error;
        return RedirectToAction(nameof(Index));
    }

    private async Task<string?> SaveLogoAsync(IFormFile? file)
    {
        if (file is null || file.Length == 0) return null;
        var folder = Path.Combine(_env.WebRootPath, "uploads", "parties");
        Directory.CreateDirectory(folder);
        var fileName = $"{Guid.NewGuid():N}{Path.GetExtension(file.FileName)}";
        var fullPath = Path.Combine(folder, fileName);
        using (var fs = System.IO.File.Create(fullPath))
            await file.CopyToAsync(fs);
        return $"/uploads/parties/{fileName}".Replace("\\", "/");
    }
}
