using EVote360.Application.DTOs.Request;
using EVote360.Application.DTOs.Response;
using EVote360.Application.Results;

namespace EVote360.Application.Abstractions.Services;

public interface IPositionService
{
    Task<Result<IEnumerable<PositionResponse>>> GetListAsync(CancellationToken ct = default);
    Task<Result<PositionResponse>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<Guid>> CreateAsync(PositionCreateRequest request, CancellationToken ct = default);
    Task<Result> UpdateAsync(Guid id, PositionUpdateRequest request, CancellationToken ct = default);
    Task<Result> ToggleActiveAsync(Guid id, CancellationToken ct = default);
}
