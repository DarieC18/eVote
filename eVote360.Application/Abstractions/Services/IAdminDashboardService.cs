using EVote360.Application.ViewModels.Admin;

namespace EVote360.Application.Abstractions.Services;

public interface IAdminDashboardService
{
    Task<AdminResumenVm> GetResumenAsync(int? year, CancellationToken ct);
}
