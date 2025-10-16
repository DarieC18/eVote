using eVote360.Application.Abstractions.Services;
using EVote360.Application.Abstractions.Repositories;
using EVote360.Application.Common.Security;
using EVote360.Domain.Entities;


namespace EVote360.Application.Services;
public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _repo;
    private readonly IPasswordHasher _hasher;
    public AuthService(IUsuarioRepository repo, IPasswordHasher hasher) { _repo = repo; _hasher = hasher; }

    public async Task<(bool ok, string error, Usuario? user)> ValidateUserAsync(string userName, string password, CancellationToken ct = default)
    {
        var user = await _repo.GetByUserNameAsync(userName, ct);
        if (user == null) return (false, "Datos de acceso inválidos.", null);
        if (!user.Activo) return (false, "El usuario está inactivo.", null);
        if (!_hasher.Verify(user.PasswordHash, password)) return (false, "Datos de acceso inválidos.", null);
        return (true, "", user);
    }

    public Task<bool> DirigentePuedeIniciarAsync(Usuario user, CancellationToken ct = default)
        => user.Rol == "Dirigente" ? _repo.DirigenteTienePartidoActivoAsync(user.Id, ct) : Task.FromResult(true);
}
