using EVote360.Application.DTOs.Candidates.Requests;
using EVote360.Application.DTOs.Candidates.Responses;

namespace EVote360.Application.Abstractions.Services;

public interface ICandidateService
{
    Task<IReadOnlyList<CandidateResponseDto>> ListByPartyAsync(Guid partyId, CancellationToken ct = default);
    Task<(bool ok, string error, Guid id)> CreateAsync(CandidateCreateRequestDto request, CancellationToken ct = default);
    Task<(bool ok, string error)> UpdateAsync(CandidateUpdateRequestDto request, CancellationToken ct = default);
    Task<(bool ok, string error)> DeleteAsync(Guid id, CancellationToken ct = default);

    Task<(bool ok, string error)> AssignToPositionAsync(Guid candidateId, Guid positionId, CancellationToken ct = default);
}
