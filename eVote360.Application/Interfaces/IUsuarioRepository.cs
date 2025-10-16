using EVote360.Domain.Entities;

namespace EVote360.Application.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> GetByUserNameAsync(string userName, CancellationToken ct = default);
        Task<bool> DirigenteTienePartidoActivoAsync(int usuarioId, CancellationToken ct = default);
    }
}
