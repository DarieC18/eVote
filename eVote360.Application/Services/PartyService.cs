using AutoMapper;
using EVote360.Application.Abstractions.Repositories;
using EVote360.Application.Abstractions.Services;
using EVote360.Application.DTOs.Request;
using EVote360.Application.DTOs.Response;
using EVote360.Application.Results;
using EVote360.Domain.Entities.Party;

namespace EVote360.Application.Services;

public sealed class PartyService : IPartyService
{
    private readonly IGenericRepository<Party> _repo;
    private readonly IMapper _mapper;

    public PartyService(IGenericRepository<Party> repo, IMapper mapper)
    {
        _repo = repo; _mapper = mapper;
    }

    public async Task<Result<IEnumerable<PartyResponse>>> GetListAsync(CancellationToken ct = default)
    {
        var items = await _repo.ListAsync(ct: ct);
        var dto = _mapper.Map<IEnumerable<PartyResponse>>(items);
        return Result<IEnumerable<PartyResponse>>.Success(dto);
    }

    public async Task<Result<PartyResponse>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(id, ct);
        return entity is null
            ? Result<PartyResponse>.Failure("Partido no encontrado.")
            : Result<PartyResponse>.Success(_mapper.Map<PartyResponse>(entity));
    }

    public async Task<Result<Guid>> CreateAsync(PartyCreateRequest request, CancellationToken ct = default)
    {
        try
        {
            var entity = new Party(request.Name, request.Siglas, request.Description);

            await _repo.AddAsync(entity, ct);
            return Result<Guid>.Success(entity.Id);
        }
        catch (ArgumentException ex)
        {
            return Result<Guid>.Failure(ex.Message);
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException)
        {
            return Result<Guid>.Failure("Ya existe un partido con las mismas siglas.");
        }
    }

    public async Task<Result> UpdateAsync(Guid id, PartyUpdateRequest request, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(id, ct);
        if (entity is null) return Result.Failure("Partido no encontrado.");

        entity.SetName(request.Name);
        entity.SetSiglas(request.Siglas);
        entity.SetDescription(request.Description);
        entity.SetLogoPath(request.LogoPath);

        try
        {
            await _repo.UpdateAsync(entity, ct);
            return Result.Success();
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException)
        {
            return Result.Failure("Ya existe un partido con esas siglas.");
        }
    }

    public async Task<Result> ToggleActiveAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(id, ct);
        if (entity is null) return Result.Failure("Partido no encontrado.");

        if (entity.IsActive) entity.Deactivate(); else entity.Activate();
        await _repo.UpdateAsync(entity, ct);
        return Result.Success();
    }
}
