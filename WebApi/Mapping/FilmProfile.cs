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
                .IncludeAllDerived()
                .MaxDepth(2);

            CreateMap<Film, FilmViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Year, opt => opt.MapFrom(src => src.Year))
                .ForMember(dest => dest.Director, opt => opt.MapFrom(src => src.Director))
                .ForMember(dest => dest.Preview, opt => opt.MapFrom(src => src.Preview.Data))
                .ForMember(dest => dest.Genres, opt => opt.MapFrom(src => src.FilmsGenres.Select(x => x.Genre)))
                .IncludeAllDerived()
                .MaxDepth(2)
                .AfterMap((film, filmVM) => 
                {
                    filmVM.GenresText = "";
                    var genres = film.FilmsGenres?.Select(x => x.Genre)?.OrderBy(x => x.Name)?.ToList();
                    if (genres?.Count > 0)
                    {
                        filmVM.GenresText += genres[0].Name;
                        for (int i = 1; i < genres.Count; i++)
                        {
                            filmVM.GenresText += ", " + genres[i].Name;
                        }
                    }
                });
        }
    }
}
