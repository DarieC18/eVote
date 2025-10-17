namespace EVote360.Application.DTOs.Request;

public sealed record VoteReceiptRequestDto(
    string ToEmail,
    string ToName,
    Guid ElectionId,
    Guid CitizenId,
    string Subject,
    string HtmlBody
);
