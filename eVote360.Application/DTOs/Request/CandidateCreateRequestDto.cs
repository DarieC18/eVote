namespace EVote360.Application.DTOs.Candidates.Requests;

public sealed record CandidateCreateRequestDto(
    Guid PartyId,
    string FirstName,
    string LastName
);
