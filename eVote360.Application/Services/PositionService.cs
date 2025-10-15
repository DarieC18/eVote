using AutoMapper;
using eVote360.Application.Abstractions.Services;
using eVote360.Application.DTOs.Request;
using eVote360.Application.Results;
using EVote360.Application.Abstractions.Repositories;
using EVote360.Application.DTOs.Request;
using EVote360.Application.DTOs.Response;
using EVote360.Domain.Entities.Position;

namespace EVote360.Application.Services;

public sealed class PositionService : IPositionService
{
    private readonly IGenericRepository<Position> _repo;
    private readonly IMapper _mapper;

    public PositionService(IGenericRepository<Position> repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<PositionResponse>>> GetListAsync(CancellationToken ct = default)
    {
        var items = await _repo.ListAsync(ct: ct);
        return Result<IEnumerable<PositionResponse>>.Success(_mapper.Map<IEnumerable<PositionResponse>>(items));
    }

    public async Task<Result<PositionResponse>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(id, ct);
        return entity is null
            ? Result<PositionResponse>.Failure("Puesto no encontrado.")
            : Result<PositionResponse>.Success(_mapper.Map<PositionResponse>(entity));
    }

    public async Task<Result<Guid>> CreateAsync(PositionCreateRequest request, CancellationToken ct = default)
    {
        try
        {
            var entity = new Position(request.Name);
            await _repo.AddAsync(entity, ct);
            return Result<Guid>.Success(entity.Id);
        }
        catch (ArgumentException ex)
        {
            return Result<Guid>.Failure(ex.Message);
        }
    }

    public async Task<Result> UpdateAsync(Guid id, PositionUpdateRequest request, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(id, ct);
        if (entity is null) return Result.Failure("Puesto no encontrado.");
        entity.SetName(request.Name);
        await _repo.UpdateAsync(entity, ct);
        return Result.Success();
    }

    public async Task<Result> ToggleActiveAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(id, ct);
        if (entity is null) return Result.Failure("Puesto no encontrado.");
        if (entity.IsActive) entity.Deactivate(); else entity.Activate();
        await _repo.UpdateAsync(entity, ct);
        return Result.Success();
    }
}
