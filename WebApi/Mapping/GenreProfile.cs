using AutoMapper;
using Core.Models;
using System;
using WebApi.ViewModels;

namespace WebApi.Mapping
{
    public class GenreProfile : Profile
    {
        public GenreProfile()
        {
            CreateMap<GenreViewModel, Genre>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .IncludeAllDerived()
                .MaxDepth(2);

            CreateMap<Genre, GenreViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .IncludeAllDerived()
                .MaxDepth(2);
        }
    }
}
