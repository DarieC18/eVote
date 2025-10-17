namespace EVote360.Application.DTOs.Candidates.Responses;

public sealed record CandidateResponseDto(
    Guid Id,
    Guid PartyId,
    string FirstName,
    string LastName
);
