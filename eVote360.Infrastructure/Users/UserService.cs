using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using EVote360.Application.Abstractions.Services;
using EVote360.Application.DTOs.Request;
using EVote360.Application.DTOs.Response;
using EVote360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EVote360.Infrastructure.Users;

public class UserService : IUserService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public UserService(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    private static string HashPassword(string plain)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(plain));
        return Convert.ToHexString(bytes);
    }

    private Task<bool> EmailExistsAsync(string email, int? exceptId, CancellationToken ct)
        => _db.Usuarios.AnyAsync(u => u.Email == email && (!exceptId.HasValue || u.Id != exceptId.Value), ct);

    private Task<bool> UserNameExistsAsync(string userName, int? exceptId, CancellationToken ct)
        => _db.Usuarios.AnyAsync(u => u.NombreUsuario == userName && (!exceptId.HasValue || u.Id != exceptId.Value), ct);

    public Task<List<UserResponseDto>> ListAsync(CancellationToken ct)
        => _db.Usuarios
            .AsNoTracking()
            .OrderBy(u => u.Apellido).ThenBy(u => u.Nombre)
            .ProjectTo<UserResponseDto>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);

    public Task<UserResponseDto?> GetAsync(int id, CancellationToken ct)
        => _db.Usuarios
            .AsNoTracking()
            .Where(u => u.Id == id)
            .ProjectTo<UserResponseDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);

    public async Task<(bool ok, string? error)> CreateAsync(UserCreateRequestDto dto, CancellationToken ct)
    {
        if (await EmailExistsAsync(dto.Email, null, ct)) return (false, "El email ya existe.");
        if (await UserNameExistsAsync(dto.UserName, null, ct)) return (false, "El usuario ya existe.");

        var hash = HashPassword(dto.Password);

        var entity = new EVote360.Domain.Entities.Usuario(
            nombre: dto.FirstName,
            apellido: dto.LastName,
            email: dto.Email,
            nombreUsuario: dto.UserName,
            passwordHash: hash,
            rol: dto.Role
        );

        _db.Usuarios.Add(entity);
        await _db.SaveChangesAsync(ct);
        return (true, null);
    }

    public async Task<(bool ok, string? error)> UpdateAsync(UserUpdateRequestDto dto, CancellationToken ct)
    {
        var entity = await _db.Usuarios.FirstOrDefaultAsync(u => u.Id == dto.Id, ct);
        if (entity is null) return (false, "Usuario no encontrado.");

        if (await EmailExistsAsync(dto.Email, dto.Id, ct)) return (false, "El email ya existe.");
        if (await UserNameExistsAsync(dto.UserName, dto.Id, ct)) return (false, "El usuario ya existe.");

        entity.SetNombre(dto.FirstName, dto.LastName);
        entity.SetEmail(dto.Email);
        entity.SetNombreUsuario(dto.UserName);
        entity.SetRol(dto.Role);

        if (!string.IsNullOrWhiteSpace(dto.NewPassword))
        {
            var newHash = HashPassword(dto.NewPassword);
            entity.SetPasswordHash(newHash);
        }

        await _db.SaveChangesAsync(ct);
        return (true, null);
    }

    public async Task<(bool ok, string? error)> ToggleActiveAsync(int id, CancellationToken ct)
    {
        var entity = await _db.Usuarios.FirstOrDefaultAsync(u => u.Id == id, ct);
        if (entity is null) return (false, "Usuario no encontrado.");

        if (entity.Activo) entity.Desactivar(); else entity.Activar();

        await _db.SaveChangesAsync(ct);
        return (true, null);
    }
}
