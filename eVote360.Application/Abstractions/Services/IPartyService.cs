using EVote360.Application.DTOs.Request;
using EVote360.Application.DTOs.Response;
using EVote360.Application.Results;

namespace EVote360.Application.Abstractions.Services;

public interface IPartyService
{
    Task<Result<IEnumerable<PartyResponse>>> GetListAsync(CancellationToken ct = default);
    Task<Result<PartyResponse>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<Guid>> CreateAsync(PartyCreateRequest request, CancellationToken ct = default);
    Task<Result> UpdateAsync(Guid id, PartyUpdateRequest request, CancellationToken ct = default);
    Task<Result> ToggleActiveAsync(Guid id, CancellationToken ct = default);
}
