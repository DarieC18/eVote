using EVote360.Application.DTOs.Request;

namespace EVote360.Application.Abstractions.Services;

public interface IEmailService
{
    Task QueueAsync(VoteReceiptRequestDto message, CancellationToken ct = default);
    Task SendAsync(VoteReceiptRequestDto message, string subject, string body, CancellationToken cancellationToken);

}
