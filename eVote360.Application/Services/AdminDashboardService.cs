using AutoMapper;
using EVote360.Application.Abstractions.Repositories;
using EVote360.Application.Abstractions.Services;
using EVote360.Application.ViewModels.Admin;

namespace EVote360.Application.Services;

public class AdminDashboardService : IAdminDashboardService
{
    private readonly IElectionRepository _elections;
    private readonly IPartyRepository _parties;
    private readonly IVoteRepository _votes;
    private readonly IMapper _mapper;

    public AdminDashboardService(
        IElectionRepository elections,
        IPartyRepository parties,
        IVoteRepository votes,
        IMapper mapper)
    {
        _elections = elections;
        _parties = parties;
        _votes = votes;
        _mapper = mapper;
    }

    public async Task<AdminResumenVm> GetResumenAsync(int? year, CancellationToken ct)
    {
        var all = await _elections.ListAsync(null, ct);
        var years = all.Select(e => e.Year).Distinct().OrderByDescending(y => y).ToList();

        if (!years.Any())
        {
            return new AdminResumenVm
            {
                SelectedYear = null,
                Years = new List<int>(),
                Elecciones = new List<EleccionResumenVm>()
            };
        }

        var selected = year ?? years.First();
        var ofYear = all.Where(e => e.Year == selected)
                        .OrderByDescending(e => e.FinishedAt ?? e.StartedAt ?? new DateTime(e.Year, 1, 1))
                        .ToList();

        var partidos = await _parties.ListAsync(ct);

        var res = new List<EleccionResumenVm>();
        foreach (var e in ofYear)
        {
            var totalVotos = await _votes.CountByElectionAsync(e.Id, ct);

            res.Add(new EleccionResumenVm
            {
                ElectionId = e.Id,
                Nombre = e.Name,
                Fecha = e.FinishedAt ?? e.StartedAt ?? new DateTime(e.Year, 1, 1),
                CantidadPartidos = partidos.Count,
                CantidadCandidatos = 0,
                TotalVotosEmitidos = totalVotos
            });
        }

        return new AdminResumenVm
        {
            SelectedYear = selected,
            Years = years,
            Elecciones = res
        };
    }
}
