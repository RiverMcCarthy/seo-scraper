using AutoMapper;
using SEOApplicationAPI.DataAccessLayer.Models;
using SEOApplicationAPI.Models;

namespace SEOApplicationAPI.Mappers
{
    public class RankingMappingProfile : Profile
    {
        public RankingMappingProfile()
        {
            CreateMap<RankingDbo, Ranking>();

            CreateMap<List<RankingDbo>, Rankings>()
            .ForMember(dest => dest.Ranks, opt => opt.MapFrom(src => src));
        }
    }
}
