using AutoMapper;
using EVote360.Application.DTOs.Request;
using EVote360.Application.DTOs.Response;
using EVote360.Domain.Entities.Citizen;
using EVote360.Domain.ValueObjects;

namespace EVote360.Application.Mappings.EntitiesAndDtos;

public class CitizenMappingProfile : Profile
{
    public CitizenMappingProfile()
    {
        CreateMap<Citizen, CitizenResponseDto>()
            .ForMember(d => d.NationalId, m => m.MapFrom(s => s.NationalId.Value))
            .ForMember(d => d.Email, m => m.MapFrom(s => s.Email != null ? s.Email.Value : null))
            .ForMember(d => d.FullName, m => m.MapFrom(s => $"{s.FirstName} {s.LastName}"));

        CreateMap<CitizenCreateRequestDto, Citizen>()
            .ConstructUsing(dto => new Citizen(
                dto.FirstName,
                dto.LastName,
                NationalId.Create(dto.NationalId),
                string.IsNullOrWhiteSpace(dto.Email) ? null : EmailAddress.Create(dto.Email)));

        CreateMap<CitizenUpdateRequestDto, Citizen>()
            .ForAllMembers(opt => opt.Ignore());
    }
}
