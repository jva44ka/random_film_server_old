using AutoMapper;
using Core.Models;
using WebApi.ViewModels;

namespace WebApi.Mapping
{
    public class LikeProfile : Profile
    {
        public LikeProfile()
        {
            CreateMap<LikeViewModel, UserFilm>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.IsLike, opt => opt.MapFrom(src => src.IsLike))
                .ForMember(dest => dest.ViewedOn, opt => opt.MapFrom(src => src.CreatedOn))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.Owner))
                .ForMember(dest => dest.Film, opt => opt.MapFrom(src => src.Film))
                .IncludeAllDerived()
                .MaxDepth(2);

            CreateMap<UserFilm, LikeViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.IsLike, opt => opt.MapFrom(src => src.IsLike))
                .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => src.ViewedOn))
                .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.Film, opt => opt.MapFrom(src => src.Film))
                .IncludeAllDerived()
                .MaxDepth(2);
        }
    }
}
