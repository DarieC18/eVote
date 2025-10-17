using EVote360.Application.Abstractions.Repositories;
using EVote360.Domain.Entities;
using EVote360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EVote360.Infrastructure.Repositories;

public sealed class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _db;
    public UsuarioRepository(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<Usuario>> ListAsync(CancellationToken ct = default)
        => await _db.Usuarios.AsNoTracking()
              .OrderBy(x => x.Apellido).ThenBy(x => x.Nombre)
              .ToListAsync(ct);

    public Task<Usuario?> GetByIdAsync(int id, CancellationToken ct = default)
        => _db.Usuarios.FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<Usuario?> GetByUserNameAsync(string userName, CancellationToken ct = default)
        => _db.Usuarios.FirstOrDefaultAsync(x => x.NombreUsuario == userName, ct);

    public Task<bool> EmailExistsAsync(string email, int? excludeId = null, CancellationToken ct = default)
        => _db.Usuarios.AsNoTracking()
           .AnyAsync(u => u.Email == email && (!excludeId.HasValue || u.Id != excludeId.Value), ct);

    public Task<bool> UserNameExistsAsync(string userName, int? excludeId = null, CancellationToken ct = default)
        => _db.Usuarios.AsNoTracking()
           .AnyAsync(u => u.NombreUsuario == userName && (!excludeId.HasValue || u.Id != excludeId.Value), ct);

    public async Task AddAsync(Usuario entity, CancellationToken ct = default)
    { _db.Usuarios.Add(entity); await _db.SaveChangesAsync(ct); }

    public async Task UpdateAsync(Usuario entity, CancellationToken ct = default)
    { _db.Usuarios.Update(entity); await _db.SaveChangesAsync(ct); }

    public Task<bool> DirigenteTienePartidoActivoAsync(int usuarioId, CancellationToken ct = default)
        => _db.PartyAssignments.AsNoTracking()
            .AnyAsync(pa => pa.UsuarioId == usuarioId && pa.Activo, ct);
    public Task<Usuario?> GetByEmailAsync(string email, CancellationToken ct = default)
        => _db.Usuarios.FirstOrDefaultAsync(x => x.Email == email, ct);
}
