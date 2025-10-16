using AutoMapper;
using EVote360.Application.Abstractions.Repositories;
using EVote360.Application.Abstractions.Services;
using EVote360.Application.Common.Security;
using EVote360.Application.DTOs.Request;
using EVote360.Application.DTOs.Response;


namespace EVote360.Application.Services;

public class UserService : IUserService
{
    private readonly IUsuarioRepository _users;
    private readonly IPasswordHasher _hasher;
    private readonly IMapper _mapper;

    public UserService(IUsuarioRepository users, IPasswordHasher hasher, IMapper mapper)
    {
        _users = users;
        _hasher = hasher;
        _mapper = mapper;
    }

    public async Task<List<UserResponseDto>> ListAsync(CancellationToken ct)
    {
        var list = await _users.ListAsync(ct);
        return list.Select(u => _mapper.Map<UserResponseDto>(u)).ToList();
    }

    public async Task<UserResponseDto?> GetAsync(int id, CancellationToken ct)
    {
        var u = await _users.GetByIdAsync(id, ct);
        return u is null ? null : _mapper.Map<UserResponseDto>(u);
    }

    public async Task<(bool ok, string? error)> CreateAsync(UserCreateRequestDto dto, CancellationToken ct)
    {
        if (await _users.EmailExistsAsync(dto.Email, null, ct)) return (false, "El email ya existe.");
        if (await _users.UserNameExistsAsync(dto.UserName, null, ct)) return (false, "El usuario ya existe.");

        var hash = _hasher.Hash(dto.Password);

        var entity = new EVote360.Domain.Entities.Usuario(
            nombre: dto.FirstName,
            apellido: dto.LastName,
            email: dto.Email,
            nombreUsuario: dto.UserName,
            passwordHash: hash,
            rol: dto.Role
        );

        await _users.AddAsync(entity, ct);
        return (true, null);
    }

    public async Task<(bool ok, string? error)> UpdateAsync(UserUpdateRequestDto dto, CancellationToken ct)
    {
        var entity = await _users.GetByIdAsync(dto.Id, ct);
        if (entity is null) return (false, "Usuario no encontrado.");

        if (await _users.EmailExistsAsync(dto.Email, dto.Id, ct)) return (false, "El email ya existe.");
        if (await _users.UserNameExistsAsync(dto.UserName, dto.Id, ct)) return (false, "El usuario ya existe.");

        entity.SetNombre(dto.FirstName, dto.LastName);
        entity.SetEmail(dto.Email);
        entity.SetNombreUsuario(dto.UserName);
        entity.SetRol(dto.Role);

        if (!string.IsNullOrWhiteSpace(dto.NewPassword))
            entity.SetPasswordHash(_hasher.Hash(dto.NewPassword));

        await _users.UpdateAsync(entity, ct);
        return (true, null);
    }

    public async Task<(bool ok, string? error)> ToggleActiveAsync(int id, CancellationToken ct)
    {
        var entity = await _users.GetByIdAsync(id, ct);
        if (entity is null) return (false, "Usuario no encontrado.");

        if (entity.Activo) entity.Desactivar(); else entity.Activar();
        await _users.UpdateAsync(entity, ct);
        return (true, null);
    }
}
