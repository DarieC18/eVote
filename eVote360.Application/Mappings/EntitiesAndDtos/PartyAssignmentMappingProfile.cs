using AutoMapper;
using EVote360.Application.DTOs.PartyAssignments;
using EVote360.Domain.Entities.Assignments;

namespace EVote360.Application.Mappings.EntitiesAndDtos;

public class PartyAssignmentMappingProfile : Profile
{
    public PartyAssignmentMappingProfile()
    {
        CreateMap<PartyAssignment, PartyAssignmentResponseDto>()
            .ForMember(d => d.UsuarioNombre, m => m.Ignore())
            .ForMember(d => d.PartyNombre, m => m.Ignore());
    }
}
