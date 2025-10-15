using AutoMapper;
using EVote360.Application.DTOs.Request;
using EVote360.Application.DTOs.Response;
using EVote360.Application.ViewModels.Position;
using DomainEntity = EVote360.Domain.Entities.Position.Position;

namespace EVote360.Application.Profiles;

public class PositionProfile : Profile
{
    public PositionProfile()
    {
        CreateMap<PositionFormVM, PositionCreateRequest>();
        CreateMap<PositionFormVM, PositionUpdateRequest>();

        CreateMap<DomainEntity, PositionResponse>();
        CreateMap<DomainEntity, PositionListVM>();

        CreateMap<PositionResponse, PositionListVM>();
    }
}
