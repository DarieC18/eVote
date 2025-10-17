using AutoMapper;
using EVote360.Application.DTOs.Response;
using EVote360.Domain.Entities.Vote;

namespace EVote360.Application.Mappings.EntitiesAndDtos
{
    public class VoteMappingProfile : Profile
    {
        public VoteMappingProfile()
        {
            CreateMap<Vote, VoteResponseDto>()
                .ForMember(d => d.VoteId, m => m.MapFrom(s => s.Id))
                .ForMember(d => d.ElectionId, m => m.MapFrom(s => s.ElectionId))
                .ForMember(d => d.CitizenId, m => m.MapFrom(s => s.CitizenId))
                .ForMember(d => d.CastAt, m => m.MapFrom(s => s.CastAt))
                .ForMember(d => d.ReceiptHash, m => m.MapFrom(s => s.ReceiptHash))
                .ForMember(d => d.Items, m => m.MapFrom(s => s.Items));  // Mapeamos la colección Items

            CreateMap<VoteItem, VoteItemDto>()
                .ForMember(d => d.ElectionBallotId, m => m.MapFrom(s => s.ElectionBallotId))
                .ForMember(d => d.BallotOptionId, m => m.MapFrom(s => s.BallotOptionId));  // Mapeamos los ítems de voto
        }
    }
}
