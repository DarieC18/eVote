using AutoMapper;
using EVote360.Application.DTOs.Response;
using EVote360.Domain.Entities;

namespace EVote360.Application.Mappings.EntitiesAndDtos;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<Usuario, UserResponseDto>()
            .ForMember(d => d.FirstName, m => m.MapFrom(s => s.Nombre))
            .ForMember(d => d.LastName, m => m.MapFrom(s => s.Apellido))
            .ForMember(d => d.UserName, m => m.MapFrom(s => s.NombreUsuario))
            .ForMember(d => d.Role, m => m.MapFrom(s => s.Rol))
            .ForMember(d => d.IsActive, m => m.MapFrom(s => s.Activo));
    }
}
