using EVote360.Application.Interfaces;
using EVote360.Domain.Entities;
using EVote360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EVote360.Infrastructure.Repositories;
public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _ctx;
    public UsuarioRepository(AppDbContext ctx) => _ctx = ctx;

    public Task<Usuario?> GetByUserNameAsync(string userName, CancellationToken ct = default)
        => _ctx.Set<Usuario>().FirstOrDefaultAsync(u => u.NombreUsuario == userName, ct);

    public Task<bool> DirigenteTienePartidoActivoAsync(int usuarioId, CancellationToken ct = default)
        => _ctx.PartyAssignments.AnyAsync(x => x.UsuarioId == usuarioId && x.Activo, ct);
}
