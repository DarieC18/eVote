using EVote360.Domain.Entities;

namespace EVote360.Application.Interfaces;
public interface IAuthService
{
    Task<(bool ok, string error, Usuario? user)> ValidateUserAsync(string userName, string password, CancellationToken ct = default);
    Task<bool> DirigentePuedeIniciarAsync(Usuario user, CancellationToken ct = default);
}
