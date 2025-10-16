using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eVote360.Web.Areas.Dirigente.Controllers
{
    [Area("Dirigente")]
    [Authorize(Roles = "Dirigente")]
    public class HomeController : Controller
    {
        public IActionResult Index() => View();
    }
}
