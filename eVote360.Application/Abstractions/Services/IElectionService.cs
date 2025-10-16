namespace eVote360.Application.Abstractions.Services;

public interface IElectionService
{
    Task<(bool ok, string error, Guid id)> CrearAsync(int year, CancellationToken ct = default);
    Task<(bool ok, string error)> ActivarAsync(Guid electionId, CancellationToken ct = default);
    Task<(bool ok, string error)> FinalizarAsync(Guid electionId, CancellationToken ct = default);

    Task<(bool ok, string error)> ConstruirBoletaAsync(Guid electionId, CancellationToken ct = default);
    Task<IReadOnlyList<(Guid id, int year, string status)>> ListAsync(CancellationToken ct = default);
}
