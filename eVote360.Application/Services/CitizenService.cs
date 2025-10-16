using AutoMapper;
using EVote360.Application.Abstractions.Repositories;
using EVote360.Application.Abstractions.Services;
using EVote360.Application.DTOs.Request;
using EVote360.Application.DTOs.Response;
using EVote360.Domain.ValueObjects;

namespace EVote360.Application.Services;

public class CitizenService : ICitizenService
{
    private readonly ICitizenRepository _citizens;
    private readonly IElectionRepository _elections;
    private readonly IMapper _mapper;

    public CitizenService(ICitizenRepository citizens, IElectionRepository elections, IMapper mapper)
    {
        _citizens = citizens;
        _elections = elections;
        _mapper = mapper;
    }

    private async Task<bool> HasActiveElectionAsync(CancellationToken ct)
        => (await _elections.GetActiveAsync(ct)) is not null;

    public async Task<List<CitizenResponseDto>> ListAsync(CancellationToken ct)
    {
        var list = await _citizens.SearchAsync(null, 0, 500, ct);
        return list.Select(c => _mapper.Map<CitizenResponseDto>(c)).ToList();
    }

    public async Task<CitizenResponseDto?> GetAsync(Guid id, CancellationToken ct)
    {
        var entity = await _citizens.GetByIdAsync(id, ct);
        return entity is null ? null : _mapper.Map<CitizenResponseDto>(entity);
    }

    public async Task<(bool ok, string? error)> CreateAsync(CitizenCreateRequestDto dto, CancellationToken ct)
    {
        if (await HasActiveElectionAsync(ct))
            return (false, "No se pueden crear ciudadanos mientras exista una elección activa.");

        var existing = await _citizens.GetByNationalIdAsync(NationalId.Create(dto.NationalId), ct);
        if (existing is not null)
            return (false, "La cédula ya existe para otro ciudadano.");

        var voNationalId = NationalId.Create(dto.NationalId);
        EmailAddress? voEmail = string.IsNullOrWhiteSpace(dto.Email) ? null : EmailAddress.Create(dto.Email);

        var entity = new EVote360.Domain.Entities.Citizen.Citizen(dto.FirstName, dto.LastName, voNationalId, voEmail);
        await _citizens.AddAsync(entity, ct);
        return (true, null);
    }

    public async Task<(bool ok, string? error)> UpdateAsync(CitizenUpdateRequestDto dto, CancellationToken ct)
    {
        if (await HasActiveElectionAsync(ct))
            return (false, "No se pueden editar ciudadanos mientras exista una elección activa.");

        var entity = await _citizens.GetByIdAsync(dto.Id, ct);
        if (entity is null) return (false, "Ciudadano no encontrado.");

        var other = await _citizens.GetByNationalIdAsync(NationalId.Create(dto.NationalId), ct);
        if (other is not null && other.Id != entity.Id)
            return (false, "La cédula ya existe para otro ciudadano.");

        entity.SetName(dto.FirstName, dto.LastName);
        EmailAddress? voEmail = string.IsNullOrWhiteSpace(dto.Email) ? null : EmailAddress.Create(dto.Email);
        entity.SetEmail(voEmail);

        await _citizens.UpdateAsync(entity, ct);
        return (true, null);
    }

    public async Task<(bool ok, string? error)> ToggleActiveAsync(Guid id, CancellationToken ct)
    {
        if (await HasActiveElectionAsync(ct))
            return (false, "No se puede activar/desactivar ciudadanos mientras exista una elección activa.");

        var entity = await _citizens.GetByIdAsync(id, ct);
        if (entity is null) return (false, "Ciudadano no encontrado.");

        if (entity.IsActive) entity.Deactivate(); else entity.Activate();
        await _citizens.UpdateAsync(entity, ct);
        return (true, null);
    }
}
