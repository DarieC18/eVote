using AutoMapper;
using EVote360.Application.ViewModels.Admin;
using EVote360.Domain.Entities.Election;

namespace EVote360.Application.Mappings.EntitiesAndDtos;

public class ElectionMappingProfile : Profile
{
    public ElectionMappingProfile()
    {
        CreateMap<Election, EleccionResumenVm>()
            .ForMember(d => d.ElectionId, m => m.MapFrom(s => s.Id))
            .ForMember(d => d.Nombre, m => m.MapFrom(s => s.Name))
            .ForMember(d => d.Fecha, m => m.MapFrom(s => s.FinishedAt ?? s.StartedAt ?? new DateTime(s.Year, 1, 1)))
            .ForMember(d => d.CantidadPartidos, m => m.Ignore())
            .ForMember(d => d.CantidadCandidatos, m => m.Ignore())
            .ForMember(d => d.TotalVotosEmitidos, m => m.Ignore());
    }
}
