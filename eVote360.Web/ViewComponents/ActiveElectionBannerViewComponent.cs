using EVote360.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EVote360.Web.ViewComponents;

public class ActiveElectionBannerViewComponent : ViewComponent
{
    private readonly AppDbContext _db;
    public ActiveElectionBannerViewComponent(AppDbContext db) => _db = db;

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var e = await _db.Elections
            .AsNoTracking()
            .OrderByDescending(x => x.StartedAt.HasValue)
            .ThenByDescending(x => x.StartedAt)
            .ThenByDescending(x => x.Year)
            .FirstOrDefaultAsync();

        if (e == null)
            return View("Default", new ActiveElectionVm { Status = "Sin elecciones", Css = "bg-secondary", Message = "No hay elecciones registradas." });

        var vm = new ActiveElectionVm
        {
            Name = e.Name,
            Year = e.Year,
            Status = e.Status.ToString(),
            Css = e.Status switch
            {
                EVote360.Domain.Enums.ElectionStatus.Active => "bg-success text-white",
                EVote360.Domain.Enums.ElectionStatus.Draft => "bg-warning",
                EVote360.Domain.Enums.ElectionStatus.Finished => "bg-secondary text-white",
                _ => "bg-light"
            },
            Message = e.Status switch
            {
                EVote360.Domain.Enums.ElectionStatus.Active => $"Elección {e.Year}: {e.Name} — ACTIVA",
                EVote360.Domain.Enums.ElectionStatus.Draft => $"Elección {e.Year}: {e.Name} — Borrador",
                EVote360.Domain.Enums.ElectionStatus.Finished => $"Elección {e.Year}: {e.Name} — Finalizada",
                _ => $"Elección {e.Year}: {e.Name}"
            }
        };

        return View("Default", vm);
    }

    public class ActiveElectionVm
    {
        public string? Name { get; set; }
        public int Year { get; set; }
        public string Status { get; set; } = "";
        public string Css { get; set; } = "";
        public string Message { get; set; } = "";
    }
}
