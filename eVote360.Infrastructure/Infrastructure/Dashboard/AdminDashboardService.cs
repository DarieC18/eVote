using EVote360.Application.Abstractions.Services;
using EVote360.Application.ViewModels.Admin;
using EVote360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EVote360.Infrastructure.Dashboard;

public class AdminDashboardService : IAdminDashboardService
{
    private readonly AppDbContext _db;
    public AdminDashboardService(AppDbContext db) => _db = db;

    public async Task<AdminResumenVm> GetResumenAsync(int? year, CancellationToken ct)
    {
        var years = await _db.Elections
            .AsNoTracking()
            .Select(e => e.Year)
            .Distinct()
            .OrderByDescending(y => y)
            .ToListAsync(ct);

        var selected = year ?? years.FirstOrDefault();

        var elecciones = await _db.Elections
            .AsNoTracking()
            .Where(e => e.Year == selected)
            .OrderByDescending(e => e.FinishedAt ?? e.StartedAt ?? new DateTime(e.Year, 1, 1))
            .Select(e => new EleccionResumenVm
            {
                ElectionId = e.Id,
                Nombre = e.Name,
                Fecha = e.FinishedAt ?? e.StartedAt ?? new DateTime(e.Year, 1, 1),

                CantidadPartidos = (
                    from ballot in _db.ElectionBallots
                    join option in _db.BallotOptions
                        on ballot.Id equals option.ElectionBallotId
                    where ballot.ElectionId == e.Id && option.PartyId != null
                    select option.PartyId
                ).Distinct().Count(),

                CantidadCandidatos = (
                    from ballot in _db.ElectionBallots
                    join option in _db.BallotOptions
                        on ballot.Id equals option.ElectionBallotId
                    where ballot.ElectionId == e.Id && option.CandidateId != null
                    select option.CandidateId
                ).Distinct().Count(),

                TotalVotosEmitidos = 0
            })
            .ToListAsync(ct);

        return new AdminResumenVm
        {
            SelectedYear = years.Any() ? selected : (int?)null,
            Years = years,
            Elecciones = elecciones
        };
    }
}
