using EVote360.Application.DTOs.Request;
using EVote360.Application.DTOs.Response;

namespace EVote360.Application.Abstractions.Services;

public interface IUserService
{
    Task<List<UserResponseDto>> ListAsync(CancellationToken ct);
    Task<UserResponseDto?> GetAsync(int id, CancellationToken ct);
    Task<(bool ok, string? error)> CreateAsync(UserCreateRequestDto dto, CancellationToken ct);
    Task<(bool ok, string? error)> UpdateAsync(UserUpdateRequestDto dto, CancellationToken ct);
    Task<(bool ok, string? error)> ToggleActiveAsync(int id, CancellationToken ct);
}
