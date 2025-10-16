using AutoMapper;
using AutoMapper.QueryableExtensions;
using EVote360.Application.Abstractions.Services;
using EVote360.Application.DTOs.Request;
using EVote360.Application.DTOs.Response;
using EVote360.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using EVote360.Domain.ValueObjects;

namespace EVote360.Infrastructure.Citizens;

public class CitizenService : ICitizenService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public CitizenService(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }
    private Task<bool> HasActiveElectionAsync(CancellationToken ct)
        => _db.Elections.AnyAsync(e => e.Status == Domain.Enums.ElectionStatus.Active, ct);

    // Valida cédula única (comparando el valor normalizado del VO)
    private Task<bool> NationalIdExistsAsync(string nationalIdValue, Guid? exceptId, CancellationToken ct)
        => _db.Citizens.AnyAsync(
            c => c.NationalId.Value == nationalIdValue &&
                 (!exceptId.HasValue || c.Id != exceptId.Value), ct);

    public Task<List<CitizenResponseDto>> ListAsync(CancellationToken ct)
        => _db.Citizens
            .AsNoTracking()
            .OrderBy(c => c.LastName).ThenBy(c => c.FirstName)
            .ProjectTo<CitizenResponseDto>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);

    public Task<CitizenResponseDto?> GetAsync(Guid id, CancellationToken ct)
        => _db.Citizens
            .AsNoTracking()
            .Where(c => c.Id == id)
            .ProjectTo<CitizenResponseDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);

    public async Task<(bool ok, string? error)> CreateAsync(CitizenCreateRequestDto dto, CancellationToken ct)
    {
        if (await HasActiveElectionAsync(ct))
            return (false, "No se pueden crear ciudadanos mientras exista una elección activa.");

        if (await NationalIdExistsAsync(dto.NationalId, null, ct))
            return (false, "La cédula ya existe para otro ciudadano.");

        // Creación de Value Objects usando sus factorías
        var voNationalId = NationalId.Create(dto.NationalId);
        EmailAddress? voEmail = string.IsNullOrWhiteSpace(dto.Email)
            ? null
            : EmailAddress.Create(dto.Email);

        var entity = new EVote360.Domain.Entities.Citizen.Citizen(
            dto.FirstName, dto.LastName, voNationalId, voEmail
        );

        _db.Citizens.Add(entity);
        await _db.SaveChangesAsync(ct);
        return (true, null);
    }

    public async Task<(bool ok, string? error)> UpdateAsync(CitizenUpdateRequestDto dto, CancellationToken ct)
    {
        if (await HasActiveElectionAsync(ct))
            return (false, "No se pueden editar ciudadanos mientras exista una elección activa.");

        var entity = await _db.Citizens.FirstOrDefaultAsync(c => c.Id == dto.Id, ct);
        if (entity is null)
            return (false, "Ciudadano no encontrado.");

        if (await NationalIdExistsAsync(dto.NationalId, dto.Id, ct))
            return (false, "La cédula ya existe para otro ciudadano.");

        entity.SetName(dto.FirstName, dto.LastName);

        EmailAddress? voEmail = string.IsNullOrWhiteSpace(dto.Email)
            ? null
            : EmailAddress.Create(dto.Email);
        entity.SetEmail(voEmail);

        await _db.SaveChangesAsync(ct);
        return (true, null);
    }

    public async Task<(bool ok, string? error)> ToggleActiveAsync(Guid id, CancellationToken ct)
    {
        if (await HasActiveElectionAsync(ct))
            return (false, "No se puede activar/desactivar ciudadanos mientras exista una elección activa.");

        var entity = await _db.Citizens.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (entity is null)
            return (false, "Ciudadano no encontrado.");

        if (entity.IsActive) entity.Deactivate(); else entity.Activate();

        await _db.SaveChangesAsync(ct);
        return (true, null);
    }
}
