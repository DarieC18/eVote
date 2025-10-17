namespace EVote360.Application.DTOs.Candidates.Requests;

public sealed record CandidateUpdateRequestDto(
    Guid Id,
    Guid PartyId,
    string FirstName,
    string LastName
);
