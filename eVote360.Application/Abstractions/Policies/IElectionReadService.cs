namespace EVote360.Application.Abstractions.Policies;

public interface IElectionReadService
{
    Task<bool> HasActiveElectionAsync(CancellationToken ct = default);
}
