using EVote360.Application.Common.Security;
using eVote360.Web.Models;
using EVote360.Application.Abstractions.Services;
using EVote360.Application.DTOs.Response;
using EVote360.Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EVote360.Web.Controllers
{
    public class CuentaController : Controller
    {
        private readonly IUserService _userService;
        private readonly IUserSession _userSession;
        private readonly IPasswordHasher _hasher;

        public CuentaController(IUserService userService, IUserSession userSession, IPasswordHasher hasher)
        {
            _userService = userService;
            _userSession = userSession;
            _hasher = hasher;
        }

        [HttpGet]
        public IActionResult Acceder(string? returnUrl = null)
        {
            if (_userSession.HasUser())
                return RedirectToAction("Index", "Home");

            return View("IniciarSesion",new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        public async Task<IActionResult> Acceder(LoginViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return View("IniciarSesion",model);

            var user = await ValidateUserCredentialsAsync(model.UserName, model.Password, ct);
            if (user == null)
                return ViewWithError(model, "Credenciales incorrectas.");

            if (!await CanUserLoginAsync(user, ct))
                return ViewWithError(model, "No tiene un partido político asignado. Por favor, contacte a un administrador.");

            await AuthenticateUserAsync(user);

            return !string.IsNullOrWhiteSpace(model.ReturnUrl)
                ? Redirect(model.ReturnUrl)
                : RedirectToHomeBasedOnRole(user.Role);
        }
        private async Task<UserResponseDto> ValidateUserCredentialsAsync(string userName, string password, CancellationToken ct)
        {
            return await _userService.ValidateUserAsync(userName, password, ct);
        }
        private async Task<bool> CanUserLoginAsync(UserResponseDto user, CancellationToken ct)
        {
            return await _userService.DirigentePuedeIniciarAsync(user, ct);
        }
        private async Task AuthenticateUserAsync(UserResponseDto user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
        }
        private IActionResult RedirectToHomeBasedOnRole(string role)
        {
            return role switch
            {
                "Administrador" => RedirectToAction("Index", "Home", new { area = "Admin" }),
                "Dirigente" => RedirectToAction("Index", "Home", new { area = "Dirigente" }),
                _ => RedirectToAction("Index", "Home")
            };
        }
        private IActionResult ViewWithError(LoginViewModel model, string errorMessage)
        {
            ModelState.AddModelError("", errorMessage);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Salir()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Denegado() => View();

        [HttpGet]
        public IActionResult Register()
        {
            var model = new RegisterViewModel();
            return View(model);
        }
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("", "Las contraseñas no coinciden.");
                return View(model);
            }

            if (await _userService.UserNameExistsAsync(model.UserName))
            {
                ModelState.AddModelError("", "El nombre de usuario ya está en uso.");
                return View(model);
            }

            if (await _userService.EmailExistsAsync(model.Email))
            {
                ModelState.AddModelError("", "El correo electrónico ya está en uso.");
                return View(model);
            }

            var user = new Usuario(
                model.FirstName,
                model.LastName,
                model.Email,
                model.UserName,
                _hasher.Hash(model.Password),
                "Dirigente"
            );

            var success = await _userService.RegisterUserAsync(user);
            if (!success)
            {
                ModelState.AddModelError("", "El registro falló. Intente nuevamente.");
                return View(model);
            }

            return RedirectToAction("IniciarSesion");
        }
    }
}
