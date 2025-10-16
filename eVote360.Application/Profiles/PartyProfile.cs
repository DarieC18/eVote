using AutoMapper;
using EVote360.Application.DTOs.Request;
using EVote360.Application.DTOs.Response;
using EVote360.Application.ViewModels.Party;
using DomainEntity = EVote360.Domain.Entities.Party.Party;

namespace EVote360.Application.Profiles;

public class PartyProfile : Profile
{
    public PartyProfile()
    {
        CreateMap<DomainEntity, PartyResponse>();
        CreateMap<DomainEntity, PartyListVM>();
        CreateMap<PartyFormVM, PartyCreateRequest>();
        CreateMap<PartyFormVM, PartyUpdateRequest>();
    }
}
