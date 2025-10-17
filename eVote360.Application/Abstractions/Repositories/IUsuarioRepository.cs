using EVote360.Domain.Entities;

namespace EVote360.Application.Abstractions.Repositories;

public interface IUsuarioRepository
{
    Task<IReadOnlyList<Usuario>> ListAsync(CancellationToken ct = default);
    Task<Usuario?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Usuario?> GetByUserNameAsync(string userName, CancellationToken ct = default);
    Task<Usuario?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<bool> EmailExistsAsync(string email, int? excludeId = null, CancellationToken ct = default);
    Task<bool> UserNameExistsAsync(string userName, int? excludeId = null, CancellationToken ct = default);
    Task AddAsync(Usuario entity, CancellationToken ct = default);
    Task UpdateAsync(Usuario entity, CancellationToken ct = default);
    Task<bool> DirigenteTienePartidoActivoAsync(int usuarioId, CancellationToken ct = default);
}
