using AutoMapper;
using EVote360.Application.DTOs.Response;
using EVote360.Domain.Entities.Position;

namespace EVote360.Application.Mappings.EntitiesAndDtos;

public class PositionMappingProfile : Profile
{
    public PositionMappingProfile()
    {
        CreateMap<Position, PositionResponse>();
    }
}
