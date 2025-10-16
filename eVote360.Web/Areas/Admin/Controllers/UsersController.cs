using AutoMapper;
using EVote360.Application.Abstractions.Services;
using EVote360.Application.DTOs.Request;
using EVote360.Application.ViewModels.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EVote360.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrador")]
    public class UsersController : Controller
    {
        private readonly IUserService _service;
        private readonly IMapper _mapper;

        public UsersController(IUserService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var list = await _service.ListAsync(ct);
            return View(list);
        }

        [HttpGet]
        public IActionResult Create() => View(new UserCreateVm());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateVm vm, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(vm);

            var dto = _mapper.Map<UserCreateRequestDto>(vm);
            var (ok, error) = await _service.CreateAsync(dto, ct);
            if (!ok)
            {
                TempData["Error"] = error;
                return View(vm);
            }

            TempData["Ok"] = "Usuario creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var dto = await _service.GetAsync(id, ct);
            if (dto is null) return NotFound();

            var vm = new UserEditVm
            {
                Id = dto.Id,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                UserName = dto.UserName,
                Role = dto.Role
            };
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserEditVm vm, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(vm);

            var dto = _mapper.Map<UserUpdateRequestDto>(vm);
            var (ok, error) = await _service.UpdateAsync(dto, ct);
            if (!ok)
            {
                TempData["Error"] = error;
                return View(vm);
            }

            TempData["Ok"] = "Usuario actualizado.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle(int id, CancellationToken ct)
        {
            var (ok, error) = await _service.ToggleActiveAsync(id, ct);
            TempData[ok ? "Ok" : "Error"] = ok ? "Estado actualizado." : error;
            return RedirectToAction(nameof(Index));
        }
    }
}
