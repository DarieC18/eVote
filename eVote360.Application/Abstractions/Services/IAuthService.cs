using EVote360.Domain.Entities;

namespace eVote360.Application.Abstractions.Services;
public interface IAuthService
{
    Task<(bool ok, string error, Usuario? user)> ValidateUserAsync(string userName, string password, CancellationToken ct = default);
    Task<bool> DirigentePuedeIniciarAsync(Usuario user, CancellationToken ct = default);
}
