using AutoMapper;
using EVote360.Application.Abstractions.Repositories;
using EVote360.Application.Abstractions.Services;
using EVote360.Application.Common.Security;
using EVote360.Application.DTOs.Request;
using EVote360.Application.DTOs.Response;
using EVote360.Domain.Entities;
using System.Security.Cryptography;
using System.Text;

namespace EVote360.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUsuarioRepository _users;
        private readonly IPartyAssignmentRepository _partyAssignments;
        private readonly IPasswordHasher _hasher;
        private readonly IMapper _mapper;

        public UserService(IUsuarioRepository users, IPartyAssignmentRepository partyAssignments, IPasswordHasher hasher, IMapper mapper)
        {
            _users = users;
            _partyAssignments = partyAssignments;
            _hasher = hasher;
            _mapper = mapper;
        }

        public async Task<List<UserResponseDto>> ListAsync(CancellationToken ct)
        {
            var users = await _users.ListAsync(ct);
            return users.Select(u => _mapper.Map<UserResponseDto>(u)).ToList();
        }

        public async Task<UserResponseDto?> GetAsync(int id, CancellationToken ct)
        {
            var user = await _users.GetByIdAsync(id, ct);
            return user is null ? null : _mapper.Map<UserResponseDto>(user);
        }

        public async Task<(bool ok, string? error)> CreateAsync(UserCreateRequestDto dto, CancellationToken ct)
        {
            if (await IsEmailOrUserNameTakenAsync(dto.Email, dto.UserName, ct))
                return (false, "El email o el nombre de usuario ya existe.");

            var hashedPassword = _hasher.Hash(dto.Password);
            var user = MapToUserEntity(dto, hashedPassword);

            await _users.AddAsync(user, ct);
            return (true, null);
        }

        public async Task<(bool ok, string? error)> UpdateAsync(UserUpdateRequestDto dto, CancellationToken ct)
        {
            var userEntity = await _users.GetByIdAsync(dto.Id, ct);
            if (userEntity is null) return (false, "Usuario no encontrado.");

            if (await IsEmailOrUserNameTakenAsync(dto.Email, dto.UserName, ct, dto.Id))
                return (false, "El email o el nombre de usuario ya existe.");

            UpdateUserEntity(userEntity, dto);
            if (!string.IsNullOrWhiteSpace(dto.NewPassword))
                userEntity.SetPasswordHash(_hasher.Hash(dto.NewPassword));

            await _users.UpdateAsync(userEntity, ct);
            return (true, null);
        }

        public async Task<(bool ok, string? error)> ToggleActiveAsync(int id, CancellationToken ct)
        {
            var userEntity = await _users.GetByIdAsync(id, ct);
            if (userEntity is null) return (false, "Usuario no encontrado.");

            if (userEntity.Activo)
                userEntity.Desactivar();
            else
                userEntity.Activar();

            await _users.UpdateAsync(userEntity, ct);
            return (true, null);
        }

        public async Task<bool> DirigentePuedeIniciarAsync(UserResponseDto user, CancellationToken ct)
        {
            if (user.Role != "Dirigente") return false;

            var partyAssignments = await _partyAssignments.GetByUserAsync(user.Id, ct);
            return partyAssignments?.Any() ?? false;
        }

        public async Task<UserResponseDto?> ValidateUserAsync(string username, string password, CancellationToken ct)
        {
            var user = await _users.GetByUserNameAsync(username, ct);
            if (user == null || !VerifyPasswordHash(password, user.PasswordHash)) return null;

            return MapToUserResponseDto(user);
        }

        private async Task<bool> IsEmailOrUserNameTakenAsync(string email, string userName, CancellationToken ct, int? userId = null)
        {
            var emailExists = await _users.EmailExistsAsync(email, userId, ct);
            var userNameExists = await _users.UserNameExistsAsync(userName, userId, ct);
            return emailExists || userNameExists;
        }

        private EVote360.Domain.Entities.Usuario MapToUserEntity(UserCreateRequestDto dto, string hashedPassword)
        {
            return new EVote360.Domain.Entities.Usuario(
                nombre: dto.FirstName,
                apellido: dto.LastName,
                email: dto.Email,
                nombreUsuario: dto.UserName,
                passwordHash: hashedPassword,
                rol: dto.Role
            );
        }

        private void UpdateUserEntity(EVote360.Domain.Entities.Usuario entity, UserUpdateRequestDto dto)
        {
            entity.SetNombre(dto.FirstName, dto.LastName);
            entity.SetEmail(dto.Email);
            entity.SetNombreUsuario(dto.UserName);
            entity.SetRol(dto.Role);
        }

        private UserResponseDto MapToUserResponseDto(EVote360.Domain.Entities.Usuario user)
        {
            return new UserResponseDto
            {
                Id = user.Id,
                UserName = user.NombreUsuario,
                Role = user.Rol,
                FullName = $"{user.Nombre} {user.Apellido}"
            };
        }
        public bool VerifyPasswordHash(string password, string storedHash)
        {
            return _hasher.Verify(storedHash, password);
        }

        public async Task<bool> UserNameExistsAsync(string userName)
        {
            var user = await _users.GetByUserNameAsync(userName);
            return user != null;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            var user = await _users.GetByEmailAsync(email);
            return user != null;
        }
        public async Task<bool> RegisterUserAsync(Usuario user)
        {
            if (await _users.UserNameExistsAsync(user.NombreUsuario))
                return false;

            if (await _users.EmailExistsAsync(user.Email))
                return false;

            await _users.AddAsync(user);
            return true;
        }
    }
}
