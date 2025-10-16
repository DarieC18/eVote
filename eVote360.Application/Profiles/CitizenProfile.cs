using AutoMapper;
using EVote360.Application.ViewModels.Citizens;
using EVote360.Application.DTOs.Request;
using EVote360.Application.DTOs.Response;

namespace EVote360.Application.Profiles
{
    public class CitizenProfile : Profile
    {
        public CitizenProfile()
        {
            CreateMap<EVote360.Domain.Entities.Citizen.Citizen, CitizenResponseDto>()
                .ForMember(d => d.NationalId, opt => opt.MapFrom(s => s.NationalId.Value))
                .ForMember(d => d.Email, opt => opt.MapFrom(s => s.Email != null ? s.Email.Value : string.Empty));

            CreateMap<EVote360.Domain.Entities.Citizen.Citizen, CitizenVm>()
                .ForMember(d => d.NationalId, opt => opt.MapFrom(s => s.NationalId.Value))
                .ForMember(d => d.Email, opt => opt.MapFrom(s => s.Email != null ? s.Email.Value : string.Empty));

            CreateMap<CitizenCreateVm, CitizenCreateRequestDto>();
            CreateMap<CitizenEditVm, CitizenUpdateRequestDto>();
        }
    }
}
