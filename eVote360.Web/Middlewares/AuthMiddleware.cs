using eVote360.Web.Helpers;
using eVote360.Web.Models;


namespace eVote360.Web.Middleware
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var userSession = context.Session.Get<UserViewModel>("User");

            if (userSession == null)
            {
                if (context.Request.Path != "/Cuenta/Acceder" && !context.Request.Path.StartsWithSegments("/Cuenta"))
                {
                    context.Response.Redirect("/Cuenta/Acceder");
                    return;
                }
            }
            else
            {
                if (userSession.Role == "Administrador" && context.Request.Path.StartsWithSegments("/Home"))
                {
                }
                else if (userSession.Role == "Elector" && context.Request.Path.StartsWithSegments("/Home"))
                {
                    // Redirigir a home del votante si está autenticado
                    context.Response.Redirect("/Votante/Inicio");
                    return;
                }
            }
            await _next(context);
        }
    }
}
