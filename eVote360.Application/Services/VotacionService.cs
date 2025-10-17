using eVote360.Application.Abstractions.Services;
using EVote360.Application.Abstractions.Repositories;
namespace EVote360.Application.Services;

public class VotacionService : IVotacionService
{
    private readonly IElectionRepository _elections;
    private readonly IVoteRepository _votes;
    private readonly ICitizenRepository _citizens;

    public VotacionService(
        IElectionRepository elections,
        IVoteRepository votes,
        ICitizenRepository citizens)
    {
        _elections = elections;
        _votes = votes;
        _citizens = citizens;
    }

    public async Task<(bool ok, string error)> ValidarElegibilidadAsync(string nationalId, CancellationToken ct = default)
    {
        var found = await _citizens.SearchAsync(nationalId, 0, 1, ct);
        if (!found.Any() || !found[0].IsActive)
            return (false, "Cédula no válida o ciudadano inactivo.");
        return (true, "");
    }

    public async Task<(bool ok, string error, Guid electionId)> ObtenerEleccionActivaAsync(CancellationToken ct = default)
    {
        var e = await _elections.GetActiveAsync(ct);
        return e is null ? (false, "No hay proceso electoral activo.", Guid.Empty) : (true, "", e.Id);
    }
    public async Task<(bool ok, string error)> RegistrarVotoAsync(
        Guid electionId, Guid citizenId, Guid positionId, Guid? candidateId, CancellationToken ct = default)
    {
        var yaVoto = await _votes.CountByCitizenInElectionAsync(electionId, citizenId, ct) > 0;

        if (yaVoto)
        {
            return (false, "Ya ha ejercido su derecho al voto en esta elección.");
        }

        var voto = new EVote360.Domain.Entities.Vote.Vote(electionId, citizenId, Guid.NewGuid().ToString("N"));
        await _votes.AddAsync(voto, ct);

        return (true, "Voto registrado correctamente.");
    }

    public Task<bool> CiudadanoCompletoVotosAsync(Guid electionId, Guid citizenId, CancellationToken ct = default)
        => Task.FromResult(false);

    public async Task<(string, string, IReadOnlyList<(string, string)>)> ResumenVotoAsync(Guid electionId, Guid citizenId, CancellationToken ct = default)
    {
        var e = await _elections.GetByIdAsync(electionId, ct);
        if (e is null) return ("", "", Array.Empty<(string, string)>());

        var folio = $"EV{DateTime.UtcNow:yyyyMMddHHmmss}";
        var html = "Gracias por votar.";
        var items = Array.Empty<(string, string)>();
        return (folio, html, items);
    }

    public Task<bool> YaEjecutoVotoAsync(Guid electionId, Guid citizenId, CancellationToken ct = default)
        => _votes.CountByCitizenInElectionAsync(electionId, citizenId, ct).ContinueWith(t => t.Result > 0, ct);

    public Task<IReadOnlyList<(Guid positionId, string positionName, int partidos, int candidatos)>>
        ConteoPorPuestoAsync(Guid electionId, CancellationToken ct = default)
        => Task.FromResult(Array.Empty<(Guid, string, int, int)>() as IReadOnlyList<(Guid, string, int, int)>);

    public Task<IReadOnlyList<(Guid positionId, string positionName)>>
        PuestosFaltantesAsync(Guid electionId, Guid citizenId, CancellationToken ct = default)
        => Task.FromResult(Array.Empty<(Guid, string)>() as IReadOnlyList<(Guid, string)>);
}
