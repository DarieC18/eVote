using System.Threading.Channels;
using EVote360.Application.DTOs.Request;

namespace EVote360.Infrastructure.Emails;

public sealed class InMemoryEmailQueue
{
    private readonly Channel<VoteReceiptRequestDto> _channel =
        Channel.CreateUnbounded<VoteReceiptRequestDto>();

    public ValueTask WriteAsync(VoteReceiptRequestDto dto, CancellationToken ct)
        => _channel.Writer.WriteAsync(dto, ct);

    public IAsyncEnumerable<VoteReceiptRequestDto> ReadAllAsync(CancellationToken ct)
        => _channel.Reader.ReadAllAsync(ct);
}
