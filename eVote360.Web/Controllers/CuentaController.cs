using EVote360.Application.Interfaces;
using EVote360.Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EVote360.Web.Controllers;

public class CuentaController : Controller
{
    private readonly IAuthService _auth;
    public CuentaController(IAuthService auth) => _auth = auth;

    [HttpGet]
    public IActionResult Acceder(string? returnUrl = null) => View(new LoginVm { ReturnUrl = returnUrl });

    [HttpPost]
    public async Task<IActionResult> Acceder(LoginVm model, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(model);

        var (ok, error, user) = await _auth.ValidateUserAsync(model.UserName, model.Password, ct);
        if (!ok || user is null) { ModelState.AddModelError("", error); return View(model); }

        var puede = await _auth.DirigentePuedeIniciarAsync(user, ct);
        if (!puede)
        {
            ModelState.AddModelError("", "No tiene un partido político asignado, por lo tanto no puede iniciar sesión. Por favor, contacte a un administrador.");
            return View(model);
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.NombreUsuario),
            new Claim(ClaimTypes.Role, user.Rol)
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

        if (!string.IsNullOrWhiteSpace(model.ReturnUrl)) return Redirect(model.ReturnUrl);
        return user.Rol == "Administrador"
            ? RedirectToAction("Index", "Home", new { area = "Admin" })
            : RedirectToAction("Index", "Home", new { area = "Dirigente" });
    }

    [HttpPost]
    public async Task<IActionResult> Salir()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    public IActionResult Denegado() => View();
}

public class LoginVm
{
    public string UserName { get; set; } = "";
    public string Password { get; set; } = "";
    public string? ReturnUrl { get; set; }
}
