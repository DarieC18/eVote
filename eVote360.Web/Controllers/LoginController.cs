using eVote360.Application.Abstractions.Services;
using eVote360.Web.Models;
using EVote360.Application.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;

namespace eVote360.Web.Controllers
{
    public class CuentaController : Controller
    {
        private readonly IUserService _userService;
        private readonly IUserSession _userSession;

        public CuentaController(IUserService userService, IUserSession userSession)
        {
            _userService = userService;
            _userSession = userSession;
        }

        [HttpGet]
        public IActionResult IniciarSesion(string? returnUrl = null)
        {
            if (_userSession.HasUser())
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        public async Task<IActionResult> IniciarSesion(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userService.ValidateUserAsync(model.UserName, model.Password, HttpContext.RequestAborted);
            if (user == null)
            {
                ModelState.AddModelError("", "Credenciales incorrectas.");
                return View(model);
            }

            _userSession.SetUserSession(user);

            if (user.Role == "Administrador")
            {
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }

            return RedirectToAction("Inicio", "Votante");
        }

        [HttpPost]
        public IActionResult Logout()
        {
            _userSession.ClearUserSession();
            return RedirectToAction("IniciarSesion", "Cuenta");
        }
    }
}
