using AutoMapper;
using EVote360.Application.DTOs.Request;
using EVote360.Application.DTOs.Response;
using EVote360.Application.ViewModels.Users;

namespace EVote360.Application.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<EVote360.Domain.Entities.Usuario, UserResponseDto>()
            .ForMember(d => d.FirstName, opt => opt.MapFrom(s => s.Nombre))
            .ForMember(d => d.LastName, opt => opt.MapFrom(s => s.Apellido))
            .ForMember(d => d.Email, opt => opt.MapFrom(s => s.Email))
            .ForMember(d => d.UserName, opt => opt.MapFrom(s => s.NombreUsuario))
            .ForMember(d => d.Role, opt => opt.MapFrom(s => s.Rol))
            .ForMember(d => d.IsActive, opt => opt.MapFrom(s => s.Activo))
            .ForMember(d => d.CreatedAt, opt => opt.Ignore());

        CreateMap<UserCreateVm, UserCreateRequestDto>();
        CreateMap<UserEditVm, UserUpdateRequestDto>();
    }
}
