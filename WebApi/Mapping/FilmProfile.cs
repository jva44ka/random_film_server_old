using AutoMapper;
using Core.Models;
using System.Linq;
using WebApi.ViewModels;

namespace WebApi.Mapping
{
    public class FilmProfile : Profile
    {
        public FilmProfile()
        {
            CreateMap<FilmViewModel, Film>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Year, opt => opt.MapFrom(src => src.Year))
                .ForMember(dest => dest.Director, opt => opt.MapFrom(src => src.Director))
                .ForMember(dest => dest.UrlImg, opt => opt.MapFrom(src => src.UrlImg))
#warning разобраться с тем как лучше мапить кростаблицу
                /*.AfterMap((src, targ) =>
                {
                    targ.FilmsGenres = new List<FilmsGenres>();
                    if (src.Genres != null)
                    {
                        foreach (var genre in src.Genres)
                            targ.FilmsGenres.Add(new FilmsGenres
                            {
                                Film = targ,
                                Genre = genre
                            });
                    }
                })*/
                .IncludeAllDerived()
                .MaxDepth(2);

            CreateMap<Film, FilmViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Year, opt => opt.MapFrom(src => src.Year))
                .ForMember(dest => dest.Director, opt => opt.MapFrom(src => src.Director))
                .ForMember(dest => dest.UrlImg, opt => opt.MapFrom(src => src.UrlImg))
                .ForMember(dest => dest.Genres, opt => opt.MapFrom(src => src.FilmsGenres.Select(x => x.Genre)))
                .IncludeAllDerived()
                .MaxDepth(2);
        }
    }
}
