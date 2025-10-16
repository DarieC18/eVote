using AutoMapper;
using EVote360.Application.DTOs.Response;
using EVote360.Domain.Entities.Party;

namespace EVote360.Application.Mappings.EntitiesAndDtos;

public class PartyMappingProfile : Profile
{
    public PartyMappingProfile()
    {
        CreateMap<Party, PartyResponse>()
            .ForMember(d => d.Acronym, m => m.MapFrom(s => s.Siglas));
    }
}
