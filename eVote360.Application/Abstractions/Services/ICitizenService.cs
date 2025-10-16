using EVote360.Application.DTOs.Request;
using EVote360.Application.DTOs.Response;

namespace EVote360.Application.Abstractions.Services;

public interface ICitizenService
{
    Task<List<CitizenResponseDto>> ListAsync(CancellationToken ct);
    Task<CitizenResponseDto?> GetAsync(Guid id, CancellationToken ct);
    Task<(bool ok, string? error)> CreateAsync(CitizenCreateRequestDto dto, CancellationToken ct);
    Task<(bool ok, string? error)> UpdateAsync(CitizenUpdateRequestDto dto, CancellationToken ct);
    Task<(bool ok, string? error)> ToggleActiveAsync(Guid id, CancellationToken ct);
}
