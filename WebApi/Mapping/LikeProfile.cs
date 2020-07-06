using AutoMapper;
using Core.Models;
using WebApi.ViewModels;

namespace WebApi.Mapping
{
    public class LikeProfile : Profile
    {
        public LikeProfile()
        {
            CreateMap<LikeViewModel, Like>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.IsLike, opt => opt.MapFrom(src => src.IsLike))
                .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => src.CreatedOn))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.Owner))
                .ForMember(dest => dest.Film, opt => opt.MapFrom(src => src.Film))
                .IncludeAllDerived()
                .MaxDepth(2);

            CreateMap<Like, LikeViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.IsLike, opt => opt.MapFrom(src => src.IsLike))
                .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => src.CreatedOn))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.Owner))
                .ForMember(dest => dest.Film, opt => opt.MapFrom(src => src.Film))
                .IncludeAllDerived()
                .MaxDepth(2);
        }
    }
}
