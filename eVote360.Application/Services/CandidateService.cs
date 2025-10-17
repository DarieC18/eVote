using EVote360.Application.Abstractions.Repositories;
using EVote360.Application.Abstractions.Services;
using EVote360.Application.DTOs.Candidates.Requests;
using EVote360.Application.DTOs.Candidates.Responses;
using EVote360.Domain.Entities;
using EVote360.Domain.Entities.Ballot;
using EVote360.Domain.Entities.Candidate;
using System.Reflection;

namespace EVote360.Application.Services;

public sealed class CandidateService : ICandidateService
{
    private readonly ICandidateRepository _candidates;
    private readonly IElectionRepository _elections;

    public CandidateService(ICandidateRepository candidates, IElectionRepository elections)
    {
        _candidates = candidates;
        _elections = elections;
    }

    public async Task<IReadOnlyList<CandidateResponseDto>> ListByPartyAsync(Guid partyId, CancellationToken ct = default)
    {
        var list = await _candidates.ListByPartyAsync(partyId, ct);
        return list.Select(c => new CandidateResponseDto(c.Id, c.PartyId, c.FirstName, c.LastName)).ToList();
    }

    public async Task<(bool ok, string error, Guid id)> CreateAsync(CandidateCreateRequestDto request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
            return (false, "Nombre y Apellido son requeridos.", Guid.Empty);

        var entity = new Candidate(request.FirstName, request.LastName, request.PartyId);
        await _candidates.AddAsync(entity, ct);
        return (true, "", entity.Id);
    }

    public async Task<(bool ok, string error)> UpdateAsync(CandidateUpdateRequestDto request, CancellationToken ct = default)
    {
        var entity = await _candidates.GetByIdAsync(request.Id, ct);
        if (entity is null) return (false, "Candidato no encontrado.");

        var updated = false;
        var miRename = entity.GetType().GetMethod("UpdateNames", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (miRename != null)
        {
            miRename.Invoke(entity, new object[] { request.FirstName, request.LastName });
            updated = true;
        }

        var miChangeParty = entity.GetType().GetMethod("ChangeParty", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (miChangeParty != null && entity.PartyId != request.PartyId)
        {
            miChangeParty.Invoke(entity, new object[] { request.PartyId });
            updated = true;
        }
        if (!updated)
        {
            SetPrivateProperty(entity, "FirstName", request.FirstName);
            SetPrivateProperty(entity, "LastName", request.LastName);
            if (entity.PartyId != request.PartyId)
                SetPrivateProperty(entity, "PartyId", request.PartyId);
        }

        await _candidates.UpdateAsync(entity, ct);
        return (true, "");
    }

    public async Task<(bool ok, string error)> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _candidates.GetByIdAsync(id, ct);
        if (entity is null) return (false, "Candidato no encontrado.");

        await _candidates.DeleteAsync(entity, ct);
        return (true, "");
    }

    public async Task<(bool ok, string error)> AssignToPositionAsync(Guid candidateId, Guid positionId, CancellationToken ct = default)
    {
        var active = await _elections.GetActiveAsync(ct);
        if (active is null) return (false, "No hay elección activa.");

        var cand = await _candidates.GetByIdAsync(candidateId, ct);
        if (cand is null) return (false, "Candidato no encontrado.");

        var yaExiste = await _elections.CandidateOptionExistsAsync(active.Id, positionId, candidateId, ct);
        if (yaExiste) return (false, "El candidato ya está asignado a ese puesto en la elección activa.");

        if (!await _elections.BallotExistsAsync(active.Id, positionId, ct))
            await _elections.AddBallotAsync(new ElectionBallot(active.Id, positionId), ct);

        var option = CreateBallotOptionCandidate(active.Id, positionId, candidateId);
        await _elections.AddOptionAsync(option, ct);

        return (true, "");
    }

    private static void SetPrivateProperty<T>(object target, string propName, T value)
    {
        var prop = target.GetType().GetProperty(propName,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (prop == null) throw new InvalidOperationException($"Propiedad '{propName}' no encontrada en {target.GetType().Name}.");

        var setMethod = prop.GetSetMethod(true);
        if (setMethod == null) throw new InvalidOperationException($"La propiedad '{propName}' no tiene set accesible.");
        setMethod.Invoke(target, new object?[] { value });
    }

    private static BallotOption CreateBallotOptionCandidate(Guid electionId, Guid positionId, Guid candidateId)
    {
        var mi = typeof(BallotOption).GetMethod("CreateCandidate",
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        if (mi != null)
        {
            var created = mi.Invoke(null, new object[] { electionId, positionId, candidateId });
            return (BallotOption)created!;
        }

        var ctor = typeof(BallotOption).GetConstructor(
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            binder: null,
            types: new[] { typeof(Guid), typeof(Guid), typeof(Guid?), typeof(Guid?), typeof(bool) },
            modifiers: null);

        if (ctor == null)
            throw new InvalidOperationException("No se encontró un ctor compatible de BallotOption.");
        return (BallotOption)ctor.Invoke(new object?[] { electionId, positionId, null, candidateId, false });
    }
}
